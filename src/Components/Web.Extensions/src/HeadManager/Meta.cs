// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Components.Web.Extensions
{
    /// <summary>
    /// A component that adds or updates meta tags contained in the HTML head element.
    /// </summary>
    public class Meta : HeadElementBase
    {
        private MetaElementState _state = default!;

        internal override object ElementKey => _state.Key;

        /// <summary>
        /// Gets or sets the "name" attribute of the HTML meta tag.
        /// </summary>
        [Parameter]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the "http-equiv" attribute of the HTML meta tag.
        /// </summary>
        [Parameter]
        public string? HttpEquiv { get; set; }

        /// <summary>
        /// Gets or sets the "content" attribute of the HTML meta tag.
        /// </summary>
        [Parameter]
        public string? Content { get; set; }

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            var key = GetKey();

            if (_state == null)
            {
                _state = new MetaElementState();
            }
            else if (!_state.Key.Equals(key))
            {
                // If the key changes, this component now represents a new meta tag.
                HeadManager.NotifyDisposed(this);
            }

            _state.Key = key;
            _state.Content = Content;

            HeadManager.NotifyChanged(this);
        }

        internal override async ValueTask ApplyAsync()
        {
            await HeadManager.SetMetaElementAsync(_state.Key, _state);
        }

        internal override async ValueTask<object> GetInitialStateAsync()
        {
            return await HeadManager.GetMetaElementAsync(_state.Key);
        }

        internal override ValueTask ResetInitialStateAsync(object initialState)
        {
            return HeadManager.SetMetaElementAsync(_state.Key, initialState);
        }

        private MetaElementKey GetKey()
            => (Name, HttpEquiv) switch
            {
                (string name, null) => new MetaElementKey("name", name),
                (null, string httpEquiv) => new MetaElementKey("http-equiv", httpEquiv),
                _ => throw new InvalidOperationException(
                    $"{GetType()} parameters must contain exactly one of {nameof(Name)} or {nameof(HttpEquiv)}.")
            };
    }
}

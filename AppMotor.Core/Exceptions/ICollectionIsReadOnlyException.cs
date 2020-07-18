﻿#region License
// Copyright 2020 - 2020 AppWeave.Core (https://github.com/skrysmanski/AppWeave.Core)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

namespace AppWeave.Core.Exceptions
{
    /// <summary>
    /// Marker interface for <see cref="CollectionIsReadOnlyException"/>
    /// and its variants. Via this interface on can catch all related
    /// exceptions in one <c>catch</c> block.
    /// </summary>
    public interface ICollectionIsReadOnlyException
    {
    }
}
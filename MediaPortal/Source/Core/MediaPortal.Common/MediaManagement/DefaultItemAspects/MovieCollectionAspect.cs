#region Copyright (C) 2007-2015 Team MediaPortal

/*
    Copyright (C) 2007-2015 Team MediaPortal
    http://www.team-mediaportal.com

    This file is part of MediaPortal 2

    MediaPortal 2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal 2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal 2. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;

namespace MediaPortal.Common.MediaManagement.DefaultItemAspects
{
  /// <summary>
  /// Contains the metadata specification of the "Collection" media item aspect which is assigned to movie media items.
  /// It describes a collection of movies in fx a trilogy.
  /// </summary>
  public static class MovieCollectionAspect
  {
    /// <summary>
    /// Media item aspect id of the collection aspect.
    /// </summary>
    public static readonly Guid ASPECT_ID = new Guid("F8D16402-D52E-475E-A5EF-E61A82E05A33");

    /// <summary>
    /// Collection name.
    /// </summary>
    public static readonly MediaItemAspectMetadata.SingleAttributeSpecification ATTR_COLLECTION_NAME =
        MediaItemAspectMetadata.CreateSingleStringAttributeSpecification("CollectionName", 200, Cardinality.Inline, true);


    public static readonly SingleMediaItemAspectMetadata Metadata = new SingleMediaItemAspectMetadata(
        ASPECT_ID, "MovieCollectionItem", new[] {
            ATTR_COLLECTION_NAME
        });

    public static readonly Guid ROLE_MOVIE_COLLECTION = new Guid("EB0F461A-E7BB-46D2-8D85-2A18122A0983");
  }
}
﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CsMerge.Core.Parsing {
  public class CsProjParser {

    public ProjectFile Parse( string name, Stream from ) {
      var doc = XDocument.Load( from );
      return Parse( name, doc );
    }

    public ProjectFile Parse( string name, XDocument doc ) {
      if ( doc == null || doc.Root == null ) {
        throw new ArgumentException( "Stream did not contain a valid XML document", "from" );
      }

      if ( doc.Root.Name.LocalName != "Project" ) {
        throw new ArgumentException( "Stream did not contain a project", "from" );
      }

      return new ProjectFile(
        name,
        doc.Root.Elements( doc.Root.Name.Namespace.GetName( "ItemGroup" ) ).Select( ParseItemGroup ).ToList().AsReadOnly()
      );
    }

    private ItemGroup ParseItemGroup( XElement itemGroupElement ) {
      return new ItemGroup( itemGroupElement.Elements().Select( ParseItem ).Where( i => i != null ).ToList().AsReadOnly() );
    }

    private Item ParseItem( XElement itemElement ) {
      var includeAttribute = itemElement.Attribute( "Include" );
      if ( includeAttribute == null ) {
        return null;
      }
      var include = includeAttribute.Value;

      var xNamespace = itemElement.Name.Namespace;

      switch ( itemElement.Name.LocalName ) {
        case "Reference":
          var specificVersionAttribute = itemElement.Attribute( "SpecificVersion" );
          var privateAttribute = itemElement.Attribute( "Private" );
          var hintPathAttribute = itemElement.Element( xNamespace.GetName( "HintPath" ) );
          var specificVersion = specificVersionAttribute == null ? (bool?) null : bool.Parse( specificVersionAttribute.Value );
          var @private = privateAttribute == null ? (bool?) null : bool.Parse( privateAttribute.Value );
          return new Reference( include, specificVersion, @private, hintPathAttribute == null ? null : hintPathAttribute.Value );
        case "ProjectReference":
          return new ProjectReference( include,
                                       Guid.Parse( itemElement.Element( xNamespace.GetName( "Project" ) ).Value ),
                                       itemElement.Element( xNamespace.GetName( "Name" ) ).Value );
        default:
          return new RawItem( itemElement, include );
      }
    }
  }
}
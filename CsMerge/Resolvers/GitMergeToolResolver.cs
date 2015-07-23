﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CsMerge.Core;
using CsMerge.Core.Parsing;

namespace CsMerge.Resolvers {

  public class GitMergeToolResolver<T> where T: class, IConflictableItem {

    private readonly string _repositoryRootDirectory;
    private readonly string _key;
    private readonly IEnumerable<XElement> _baseElements;
    private readonly IEnumerable<XElement> _localElements;
    private readonly IEnumerable<XElement> _incomingElements;

    public GitMergeToolResolver( string repositoryRootDirectory, Conflict<T> conflict ) {
      _key = conflict.Key;
      _repositoryRootDirectory = repositoryRootDirectory;
      _baseElements = new[] { conflict.Base.ToElement( "" ) };
      _localElements = new[] { conflict.Local.ToElement( "" ) };
      _incomingElements = new[] { conflict.Incoming.ToElement( "" ) };
    }

    public GitMergeToolResolver( string repositoryRootDirectory, Conflict<IEnumerable<T>> conflict ) {
      _key = conflict.Key;
      _repositoryRootDirectory = repositoryRootDirectory;
      _baseElements = conflict.Base.WhereNotNull().Select( i => i.ToElement( "" ) );
      _localElements = conflict.Local.WhereNotNull().Select( i => i.ToElement( "" ) );
      _incomingElements = conflict.Incoming.WhereNotNull().Select( i => i.ToElement( "" ) );
    }

    public T Resolve() {
      var xmlElement = GitHelper.ResolveWithStandardMergetool(
        _repositoryRootDirectory,
        _key,
        _baseElements,
        _localElements,
        _incomingElements
        );

      if ( xmlElement == null ) {
        return null;
      }

      var resolved = xmlElement.ParseAsConflictableItem() as T;

      if ( resolved != null ) {
        return resolved;
      }

      throw new InvalidResolutonException( _key );
    }
  }
}

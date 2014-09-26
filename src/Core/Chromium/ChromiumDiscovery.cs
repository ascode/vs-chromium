﻿// Copyright 2013 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System.Collections.Generic;
using System.Linq;
using VsChromium.Core.Configuration;
using VsChromium.Core.Files;
using VsChromium.Core.Files.PatternMatching;

namespace VsChromium.Core.Chromium {
  public class ChromiumDiscovery : IChromiumDiscovery {
    private readonly IFileSystem _fileSystem;
    private readonly IPathPatternsFile _chromiumEnlistmentPatterns;

    public ChromiumDiscovery(IConfigurationSectionProvider configurationSectionProvider, IFileSystem fileSystem) {
      _fileSystem = fileSystem;
      _chromiumEnlistmentPatterns = new PathPatternsFile(configurationSectionProvider, ConfigurationFilenames.ChromiumEnlistmentDetectionPatterns);
    }

    public void ValidateCache() {
      // Nothing to do
    }

    public FullPath GetEnlistmentRoot(FullPath filename) {
      var directory = filename.Parent;
      if (!_fileSystem.DirectoryExists(directory))
        return default(FullPath);

      return EnumerateParents(filename)
        .FirstOrDefault(x => IsChromiumSourceDirectory(x, _chromiumEnlistmentPatterns));
    }

    private IEnumerable<FullPath> EnumerateParents(FullPath path) {
      var directory = path.Parent;
      for (var parent = directory; parent != default(FullPath); parent = parent.Parent) {
        yield return parent;
      }
    }

    private bool IsChromiumSourceDirectory(FullPath path, IPathPatternsFile chromiumEnlistmentPatterns) {
      // We need to ensure that all pattern lines are covered by at least one file/directory of |path|.
      var entries = _fileSystem.GetDirectoryEntries(path);
      return chromiumEnlistmentPatterns.GetPathMatcherLines()
        .All(item => MatchFileOrDirectory(item, entries));
    }

    private static bool MatchFileOrDirectory(IPathMatcher item, DirectoryEntries entries) {
      return
        entries.Directories.Any(d => item.MatchDirectoryName(new RelativePath(d), SystemPathComparer.Instance)) ||
        entries.Files.Any(f => item.MatchFileName(new RelativePath(f), SystemPathComparer.Instance));
    }
  }
}

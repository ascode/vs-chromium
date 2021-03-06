// Copyright 2015 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System.Collections.Generic;
using VsChromium.Core.Collections;

namespace VsChromium.Features.SourceExplorerHierarchy {
  public class FileNodeViewModel : NodeViewModel {
    protected override IList<NodeViewModel> ChildrenImpl {
      get { return ArrayUtilities.EmptyList<NodeViewModel>.Instance; }
    }
  }
}
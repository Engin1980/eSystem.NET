using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo
{
  class Testing
  {
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private string _FileNamePath;
    ///<summary>
    /// Sets/gets FileNamePath value.
    ///</summary>
    public string FileNamePath
    {
      get
      {
        return (_FileNamePath);
      }
      set
      {
        _FileNamePath = value;
      }
    }
  }
}

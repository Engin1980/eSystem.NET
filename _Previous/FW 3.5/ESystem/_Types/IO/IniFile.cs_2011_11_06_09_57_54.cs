using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

//This namespace is for easy INI reading ad writing, designed to be practically errorless, you dont need to worry about things like if .exists .add .set etc, just .set 
//XML is similar to INI, but not as simple and easily modified by humans 
//INI is ideal for server settings etc. 

//any keys found NOT under a group header will be put in group MAIN 

//WARNING: THIS INI FILE DOES NOT SUPPORT COMMENTS 
//FORMAT IS AS FOLLOWS: 
//[GROUP] 
//item1=True 
//item2=some text here 
//[GROUP2] 
//soemthing=3 
using System.IO;

namespace ESystem.IO
{
  /// <summary>
  /// A class to handle all file IO for multiple savable settings 
  /// This class will create on-demand, an INI file of settings, keys need not be added before being assigned 
  /// </summary>
  /// <remarks></remarks>
  public class INIFile
  {
    #region Nested

    //This sub-class is used by the INIFile class, it handles the keys and values within an INI group 
    public class INIGroup
    {
      //this class handles the settings of values 
      protected INIFile owner;
      public List<string> inikeys;
      public List<string> inivalues;
      public INIGroup(INIFile parent)
      {
        owner = parent;
        inikeys = new List<string>();
        inivalues = new List<string>();
      }

      public void setValue(string key, string value)
      {
        key = key.ToLower();
        int i = inikeys.IndexOf(key);
        if (i == -1)
        {
          inikeys.Add(key);
          inivalues.Add(value);
        }
        else
        {
          inivalues[i] = value;
        }
        owner.SaveFile();
      }

      public string getValue(string key)
      {
        key = key.ToLower();
        int i = inikeys.IndexOf(key);
        if (i == -1)
        {
          return string.Empty;
        }
        else
        {
          return inivalues[i];
        }
      }

      public string getKey(int index)
      {
        if (index > -1 && index < getCount())
        {
          return inikeys[index];
        }
        else
        {
          //out of range 
          throw new IndexOutOfRangeException("Index was out of range, must be non-negative and less than total key count");
        }
      }

      public string getValue(int index)
      {
        if (index > -1 && index < getCount())
        {
          return inivalues[index];
        }
        else
        {
          //out of range 
          throw new IndexOutOfRangeException("Index was out of range, must be non-negative and less than total key count");
        }
      }

      public int getCount()
      {
        return inikeys.Count;
      }

    }

    #endregion Nested

    //this class handles the saving and loading of the file 
    protected string fileName;
    protected List<string> iniGroupNames;

    protected List<INIGroup> iniGroups;
    public INIFile(string filename)
    {
      fileName = filename;
      iniGroups = new List<INIGroup>();
      iniGroupNames = new List<string>();
      ReloadFile();
    }

    public INIGroup Group(string name)
    {
      name = name.ToUpper();
      int i = iniGroupNames.IndexOf(name);
      if (i == -1)
      {
        return null;
      }
      else
      {
        return iniGroups[i];
      }
    }

    public void SetValue(string group, string key, string value)
    {
      AddGroup(group);
      //create the group (if not already existing) 
      //set the value 
      Group(group).setValue(key, value);
    }

    public string GetValue(string group, string key)
    {
      group = group.ToUpper();
      key = key.ToLower();
      if (iniGroupNames.IndexOf(group) == -1)
      {
        return string.Empty;
      }
      else
      {
        return Group(group).getValue(key);
      }
    }

    public void AddGroup(string group)
    {
      group = group.ToUpper();
      if (iniGroupNames.IndexOf(group) == -1)
      {
        iniGroupNames.Add(group);
        iniGroups.Add(new INIGroup(this));
      }
    }

    public bool SaveFile()
    {
      //bool returned indicates the success of the save process 
      try
      {
        FileStream strm = new FileStream(fileName, FileMode.Create, FileAccess.Write);
        StreamWriter W = new StreamWriter(strm);
        if (iniGroupNames.Count > 0)
        {
          for (int i = 0; i <= iniGroupNames.Count - 1; i++)
          {
            INIGroup grp = iniGroups[i];
            if (grp.getCount() > 0)
            {
              //only save this group if it has content 
              W.WriteLine("[" + iniGroupNames[i] + "]");
              for (int k = 0; k <= grp.getCount() - 1; k++)
              {
                W.WriteLine((grp.getKey(k) + "=") + grp.getValue(k));
              }
              W.WriteLine(" ");
            }
          }
        }
        W.Close();
        strm.Close();
        return true;
      }
      catch
      {
        return false;
      }
    }

    public void ReloadFile()
    {
      iniGroups.Clear();
      iniGroupNames.Clear();

      if (File.Exists(fileName))
      {
        string strline = null;
        string currentGroup = "MAIN";
        FileStream strm = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        StreamReader R = new StreamReader(strm);
        while (R.EndOfStream != true)
        {
          strline = R.ReadLine();
          //get line 
          if (strline.StartsWith("["))
          {
            //this is a group header 
            currentGroup = strline.Substring(1, strline.Length - 2);
          }
          else
          {
            //this is a key=value entry 
            string[] keyval = strline.Split('=');
            if (keyval.Length == 2)
            {
              SetValue(currentGroup, keyval[0], keyval[1]);
            }
          }
        }
        R.Close();
        strm.Close();
      }
    }
  }

  
}

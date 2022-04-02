using System.IO;
using System.Runtime.InteropServices;
using System.Text;

class IniFile {

    private string path;
    private int buffer_size;

    public IniFile(string _path, int _buffer_size = 1048576) {
        path = _path;
        buffer_size = _buffer_size;

        Directory.CreateDirectory(Path.GetDirectoryName(path));
    }

    [DllImport("kernel32", CharSet = CharSet.Auto)]
    private static extern long WritePrivateProfileString(string _section, string _key, string _value, string _path);

    [DllImport("kernel32", CharSet = CharSet.Auto)]
    private static extern int GetPrivateProfileString(string _section, string _key, string _default, StringBuilder _returned_string, int _size, string _path);

    [DllImport("kernel32", CharSet = CharSet.Auto)]
    private static extern int GetPrivateProfileSection(string _section, System.IntPtr _returned_string, int _size, string _path);

    [DllImport("kernel32", CharSet = CharSet.Auto)]
    private static extern int GetPrivateProfileSectionNames(System.IntPtr _returned_string, int _size, string _path);

    public string Read(string _key, string _section, string _default = "") {
        StringBuilder _temp = new StringBuilder(buffer_size);
        GetPrivateProfileString(_section, _key, _default, _temp, buffer_size, path);
        return _temp.ToString();
    }

    public void Write(string _key, string _value, string _section) => WritePrivateProfileString(_section, _key, _value, path);

    public string[] GetAllSections() {
        System.IntPtr _mem = Marshal.AllocHGlobal(4096 * sizeof(char));
        string _temp = string.Empty;

        int count = GetPrivateProfileSectionNames(_mem, buffer_size * sizeof(char), path) - 1;
        if (count > 0) _temp = Marshal.PtrToStringUni(_mem, count);
        Marshal.FreeHGlobal(_mem);

        return _temp.Split('\0');
    }

    public string[] GetSectionContent(string _section) {
        System.IntPtr _mem = Marshal.AllocHGlobal(4096 * sizeof(char));
        string _temp = string.Empty;

        int _count = GetPrivateProfileSection(_section, _mem, buffer_size * sizeof(char), path) - 1;
        if (_count > 0) _temp = Marshal.PtrToStringUni(_mem, _count);
        Marshal.FreeHGlobal(_mem);

        return _temp.Split('\0');
    }

    public bool KeyExists(string _key, string _section) => Read(_key, _section).Length != 0;

    public void DeleteKey(string _key, string _section) => Write(_key, null, _section);

    public void DeleteSection(string _section) => Write(null, null, _section);

    public void Delete() {
        if (File.Exists(path)) File.Delete(path);
    }

}
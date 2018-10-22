
/// <summary>
/// 创建指定路径下文件夹
/// </summary>
/// <param name="path">路径</param>
public void SafeCreateDirectory(string path)
{
	if (!System.IO.Directory.Exists(path))
	{
		System.IO.Directory.CreateDirectory(path);
        Debug.WriteLine(string.Format("create time at-{0} | create folder at-{1}", DateTime.Now.ToString(), path));
	}
}

/// <summary>
/// 复制指定路径下文件到目标文件夹
/// </summary>
/// <param name="path">源路径</param>
/// <param name="destination">目标路径</param>
public void SafeCopyDirectory(string path, string destination)
{
    Debug.WriteLine(string.Format("source path-{0}", path));
    Debug.WriteLine(string.Format("destination path-{0}", destination));
	string [] files =System.IO.Directory.GetFiles(path);
    
    for (int i=0; i < files.Length; i++)
    {  
        System.IO.FileInfo file = new System.IO.FileInfo(files[i]);   
        file.CopyTo(destination + "\\" + file.Name, true);
        Debug.WriteLine(string.Format("copy time at-{0} | destination path-{1}", DateTime.Now.ToString(), destination + "\\" + file.Name));
    }
}

/// <summary>
/// 复制文件到指定路径
/// </summary>
/// <param name="path">源路径</param>
/// <param name="destination">目标路径</param>
public void SafeCopyFile(string path, string destination)
{
    Debug.WriteLine(string.Format("source path-{0}", path));
	System.IO.FileInfo files = new System.IO.FileInfo(path);
	files.CopyTo(destination, true);
}

/// <summary>
/// 复制指定路径下文件夹到指定目录中
/// </summary>
/// <param name="source">源路径</param>
/// <param name="target">目标路径</param>
public void SafeCopyFiles(string source, string target)
{
    // 如果目标目录存在
    if (System.IO.Directory.Exists(target))
    {
        // 目标目录直接删除
        System.IO.Directory.Delete(target, true);
        Debug.WriteLine(string.Format("delete target path-{0}", target));
        // 删除后，重新创建目标文件夹
        System.IO.Directory.CreateDirectory(target);
        Debug.WriteLine(string.Format("Create target path-{0}", target));
    }
    else
    {
        // 目标文件夹不存在，创建文件夹
        System.IO.Directory.CreateDirectory(target);
        Debug.WriteLine(string.Format("Create target path-{0}", target));
    }
    
    // 取得当前文件夹内全部的文件夹列表
    System.IO.DirectoryInfo array = new System.IO.DirectoryInfo(source);
    // 获取该文件夹下的文件列表
    System.IO.FileInfo[] files = array.GetFiles();
    // 获取该文件夹下的文件夹列表
    System.IO.DirectoryInfo[] Directorys = array.GetDirectories();
    
    foreach (System.IO.FileInfo file in files)//逐个复制文件
    {
        System.IO.File.Copy(source + "\\" + file.Name, target + "\\" + file.Name);
        Debug.WriteLine(string.Format("Copy to target path-{0}", target + "\\" + file.Name));
    }
    
    foreach (System.IO.DirectoryInfo Dir in Directorys)//逐个获取文件夹名称，并递归调用方法本身
    {
        SafeCopyFiles(source + "\\" + Dir.Name, target + "\\" + Dir.Name);
    }
}
		
/// <summary>
/// 是否为选择使用的数据表
/// </summary>
/// <param name="table">源表</param>
/// <param name="excludedTables">集合</param>
public bool ShouldUseTable(TableSchema table, TableSchemaCollection excludedTables)
{
	bool useTable = true;
	
	if(excludedTables != null)
	{
		if(excludedTables.Count > 0)
		{
			useTable = !excludedTables.Contains(table);
		}
	}	
	return useTable;
}

/// <summary>
/// 取得类名称
/// </summary>
/// <param name="table">源表</param>
/// <param name="suffix">后缀名</param>
public string GetClassName(TableSchema table, string suffix)
{
	return GetClassName(table,suffix,2);
}

/// <summary>
/// 取得类名称
/// </summary>
/// <param name="table">源表</param>
/// <param name="suffix">后缀名</param>
/// <param name="prefix">前缀截取长度</param>
public string GetClassName(TableSchema table, string suffix, int prefix)
{
	if(table.ExtendedProperties["NSFx_EntityName"] != null) 
	{
		return (string)table.ExtendedProperties["NSFx_EntityName"].Value;
	}

	string className = table.Name;
	
	if(StringUtil.IsPlural(className))
	{
		className = StringUtil.ToSingular(className);
	}
    //原有的替换规则被我改了 className = className.Replace("_", "");
    className = className.Replace(" ", "").Replace("_", "_");
	
    if(!className.Substring(0,3).ToUpper().Equals("SYS"))
    {
        if(prefix > 0) 
        {
            className = className.Substring(prefix);
        }
        className = className.Substring(0,1).ToUpper() + className.Substring(1);
    }
    else
    {
        //原有的替换规则被我改了 className = className.Replace("_", "");
        className = className.Replace("_", "_");
    }
    className += suffix;
    
	return className;
}

/// <summary>
/// 取得字段名称
/// </summary>
/// <param name="cs">字段名</param>
public string GetDeclarationName(ColumnSchema cs)
{
	string convert = "";
	string[] temp = cs.Name.Split('_');
	
	for(int i =0; i< temp.Length; i++)
	{
		convert += temp[i].ToLower();
	}
	return "_" + convert;
}

/// <summary>
/// 取得字段数据类型
/// </summary>
/// <param name="column">字段名</param>
public string GetCSharpVariableType(ColumnSchema column)
{
	string allowDBNullFlag = column.AllowDBNull?"?":"";
    switch (column.DataType)
	{
		case DbType.AnsiString: return "string";
		case DbType.AnsiStringFixedLength: return "string";
		case DbType.Binary: return "byte[]";
		case DbType.Boolean: return "bool" + allowDBNullFlag;
		case DbType.Byte: return "byte" + allowDBNullFlag;
		case DbType.Currency: return "decimal" + allowDBNullFlag;
		case DbType.Date: return "DateTime" + allowDBNullFlag;
		case DbType.DateTime: return "DateTime" + allowDBNullFlag;
		case DbType.Decimal: return "decimal" + allowDBNullFlag;
		case DbType.Double: return "double" + allowDBNullFlag;
		case DbType.Guid: return "Guid";
		case DbType.Int16: return "short" + allowDBNullFlag;
		case DbType.Int32: return "int" + allowDBNullFlag;
		case DbType.Int64: return "long" + allowDBNullFlag;
		case DbType.Object: return "object";
		case DbType.SByte: return "sbyte" + allowDBNullFlag;
		case DbType.Single: return "float" + allowDBNullFlag;
		case DbType.String: return "string";
		case DbType.StringFixedLength: return "string";
		case DbType.Time: return "TimeSpan";
		case DbType.UInt16: return "ushort" + allowDBNullFlag;
		case DbType.UInt32: return "uint" + allowDBNullFlag;
		case DbType.UInt64: return "ulong" + allowDBNullFlag;
		case DbType.VarNumeric: return "decimal" + allowDBNullFlag;
		case DbType.Xml: return "string";
      
		default:
		{
			return "__UNKNOWN__" + column.NativeType;
		}
	}
}

/// <summary>
/// 取得字段数据类型
/// </summary>
/// <param name="column">字段名</param>
public string GetDBColumnType(ColumnSchema column)
{
	string type = string.Empty;
    if(column.NativeType.ToUpper().Equals("VARCHAR")||
    column.NativeType.ToUpper().Equals("NCHAR")||
    column.NativeType.ToUpper().Equals("CHAR"))
    {
        type = column.NativeType + "(" + column.Size.ToString() +  ")";
    }
    else
    {
        type = column.NativeType;
    }
    
	if(!column.AllowDBNull)
    {
        type += " NOT NULL";
    }
    
    return type;
}

/// <summary>
/// 取得属性名称
/// </summary>
/// <param name="cs">字段名</param>
public string GetPropertieName(ColumnSchema cs)
{
	//string convert = "";
	//string[] temp = cs.Name.Split('_');
	
	//for(int i =0; i< temp.Length; i++)
	//{
	//	convert += temp[i].ToUpper().Substring(0,1) + temp[i].ToLower().Substring(1);
	//}
	//return convert;
	return cs.Name.Trim();
}

public string GetRegularExpressionAttr(ColumnSchema cs)
{
    string pattern = "";    
    if ((getExtentName(cs.Name, 3) =="tel")||(getExtentName(cs.Name, 6)=="mobile"))
    {
         pattern = "0{0,1}1[3|4|5|6|7|8|9][0-9]{9}"; 
    }
        
    if (getExtentName(cs.Name, 4)=="mail")
    {
        pattern = "[A-Za-z0-9.%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}";
    }

    if (getExtentName(cs.Name, 5)=="phone")
    {
         pattern = "d{3}-d{8}|d{4}-d{7}";
    }
    return pattern;
    
}

public string getExtentName(string name, int size)
{
    string extName = "";
    if (name.Length >= size)
    {        
        extName = name.Substring(name.Length-size, size).ToLower();        
    }
    return extName;
}

public string GetForeignClass(string val)
{
    return val.Substring(0,1).ToUpper()+val.Substring(1,val.Length-1).Replace("id","").Replace("Id","").Replace("ID","");
}

public bool isImageField(ColumnSchema cs)
{
    string val = cs.Name.Trim().ToUpper();
    return val.Contains("IMAGE") || val.Contains("PICTURE") || val.Contains("THUMB");
}

public int GetColspan(ColumnSchema cs)
{
    if ((cs.Size>50) || isImageField(cs))
    {
        return 3;
    }
    return 1;
}

//返回转成小写字符
public string GetParamName(string tableName){

   string temp = tableName[0].ToString().ToLower() + tableName.Remove(0,1);
   return temp;
    
}

//返回转成大写字符
public string GetToUpperName(string tableName){

   string temp = tableName[0].ToString().ToUpper() + tableName.Remove(0,1);
   return temp;
    
}

//返回表说明
public string GetTableDescriptionName(string tableDescription){
   
      string temp ="";
      if (tableDescription.Contains("表")) {
          
          temp = tableDescription.Replace("表", "");
          
         }else{
         
         temp=tableDescription;
         }
      
   return temp;
    
}

//替换字符“—” 替换下划线
   public string ReplaceString(string DataString) {

            return DataString.Replace("_", "");
        }


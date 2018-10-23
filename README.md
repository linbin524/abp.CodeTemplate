# abp.CodeTemplate
需要最新源码，或技术提问，请加QQ群：538327407，由于源码在不断完善，会在之后同步到开源项目中

简介

用abp 框架快两年了，用它完成了多个项目，作为CTO同时也作为架构师，在应对中小型项目时候，我们通常选择ABP（内部大型的物联网架构采用自己的框架），感觉这款框架真心不错。虽然开源社区有也有很多写了几套代码生成器，但是我用完之后，总是感觉不能达到我自己想要的效果，我个人还是比较喜欢一步到位，批量生成，所以就写了这套基于codesmith的代码生成器，这一套在项目中还算稳定。

 

模板介绍 

先看一下代码结构



 

 我们的项目中我规划使用的是spa的，所以一般会生成常规 四个目录，分别是如下

 



 

 

其余的中英文，还有权限、以及DbContext 部分相对数量比较少，统一改造，生成单个文件进行copy。

最后使用TemplateBuid 自动生成上面的批量文件。

 

代码解析和使用

每个代码生成器部分需要先配置对应的项目名称，和model等，细节需要自己去了解

 



 

 常规简单操作

 

一般需要我们用powerdesign等设计工具，设计好对应的表，标注要注释，先临时生成一个数据库，通过codesmith 生成代码后，在通过code first 形式，真正在abp 对应的数据库中生成数据库表。

 以下文件是TemplateBuid.Cst 文件，配置完成后，



 

生成代码操作，先编译，后生成。



 

 详细代码举例说明

 

由于篇幅有限，我就简单说明一下Repository、AppAuthorizationProvider、view中的createOrEditModal 进行简单说明

 

1、repository 

常规封装增删改查等操作，我在项目中重写了基类方法，封装了批量等操作，但没有和代码生成器组合起来，常规的业务里面不需要批量操作

 

其中 默认主键都是位ID，如果实际项目中有需求，主键要为其他字段，需要手动修改。目前我封装的主要针对ID 是int 类型、GUid 类型做了不同代码输出

复制代码
  1 <%@ CodeTemplate Language="C#" TargetLanguage="C#" ResponseEncoding="UTF-8" Description="Generates a single entity business class." Debug="True" %>
  2 <%@ Property Name="SourceTable" Type="SchemaExplorer.TableSchema" Category="2.数据库" Description="Database table that this entity should be based on." %>
  3 <%@ Property Name="TablePrefixes" Type="String" Default="" Optional="True" Category="2.数据库" Description="The table prefix to be cut from the class name" %>
  4 <%@ Property Name="RootNamespace" Type="String" Default="HashBlockChain" Optional="False" Category="1.名称空间" Description="系统名称空间的根名称." %>
  5 <%@ Property Name="Namespace" Type="String" Default="ZLDB_Domain" Optional="False" Category="1.名称空间" Description="系统当前所属文件夹的名称（命名空间相关）." %>
  6 <%@ Property Name="FolderNamespace" Type="String" Default="" Optional="true" Category="1.名称空间" Description="系统名称空间的Model名称." %>
  7 <%@ Property Name="PrefixLength" Type="Int32" Default="0" Optional="False" Category="2.数据库" Description="数据表前缀截取长度." %>
  8 <%@ Assembly Name="SchemaExplorer" %>
  9 <%@ Assembly Name="CodeSmith.BaseTemplates" %>
 10 <%@ Assembly Name="System.Data" %>
 11 <%@ Import Namespace="SchemaExplorer" %>
 12 <%@ Import Namespace="System.Data" %>
 13 <%string tableClass=GetClassName(SourceTable, "", 0); %>
 14 <% FolderNamespace=SourceTable.Name.ToString();%>
 15 <%string tableName=SourceTable.Name.ToString(); %>
 16 <%string paramName=GetParamName(tableName); %>
 17 
 18 using Abp.Application.Services;
 19 using Abp.Application.Services.Dto;
 20 using Abp.AutoMapper;
 21 using Abp.Domain.Repositories;
 22 using Abp.Domain.Uow;
 23 using AutoMapper;
 24 using System;
 25 using System.Collections.Generic;
 26 using System.Data.Entity;
 27 using System.Linq;
 28 using System.Linq.Expressions;
 29 using System.Text;
 30 using System.Threading.Tasks;
 31 using System.Linq.Dynamic;
 32 using Abp.Linq.Extensions;
 33 using <%= RootNamespace %>.<%=Namespace%>.Dtos;
 34 using <%= RootNamespace %>.Dto;
 35 using <%= RootNamespace %>.Authorization.<%=tableName%>.Exporting;
 36 namespace <%= RootNamespace %>.<%=Namespace%>
 37 {
 38 
 39  <% foreach (ColumnSchema column in SourceTable.Columns) { %>            
 40          <% if (column.IsPrimaryKeyMember) {  %>
 41          
 42           <%string tempType = GetCSharpVariableType(column); %>
 43          
 44           <% if (tempType=="Guid") {  %>
 45     /// <summary>
 46     /// <%=SourceTable.Description%> 业务实现接口
 47     /// </summary>
 48     public class <%=tableName%>AppService : AbpZeroTemplateAppServiceBase, I<%=tableName%>AppService
 49     {
 50         private readonly IRepository<<%=tableName%>, Guid> _<%=paramName%>Repository;
 51         private readonly I<%=tableName%>ListExcelExporter _i<%=tableName%>ListExcelExporter;
 52 
 53         /// <summary>
 54         /// 构造函数自动注入我们所需要的类或接口
 55         /// </summary>
 56         public <%=tableName%>AppService(IRepository<<%=tableName%>, Guid> <%=paramName%>Repository,I<%=tableName%>ListExcelExporter i<%=tableName%>ListExcelExporter)
 57         {
 58             _<%=paramName%>Repository = <%=paramName%>Repository;
 59             _i<%=tableName%>ListExcelExporter = i<%=tableName%>ListExcelExporter;
 60          
 61         }
 62 
 63         /// <summary>
 64         /// 获取所有数据列表
 65         /// </summary>
 66         /// <returns>返回数据集合</returns>
 67         public async Task<List<<%=tableName%>Dto>> GetAllList()
 68         {
 69             //调用Task仓储的特定方法GetAllWithPeople
 70             var resultList = await _<%=paramName%>Repository.GetAllListAsync();
 71             return Mapper.Map<List<<%=tableName%>Dto>>(resultList).ToList();
 72         }
 73         
 74         /// <summary>
 75         /// 获取分页数据列表 分页具体代码需要适当修改，如orderby 需要匹配 创建时间 或者其他数据Id（int）
 76         /// </summary>
 77         /// <returns>返回数据集合</returns>
 78         public async Task<PagedResultDto<<%=tableName%>Dto>> GetPagedListAsync(PagedAndFilteredInputDto input)
 79         {
 80             var query = _<%=paramName%>Repository.GetAll();
 81             //TODO:根据传入的参数添加过滤条件
 82 
 83             var resultCount = await query.CountAsync();
 84             var result<%=paramName%> = await query
 85             .OrderBy(x=>x.Id)
 86             .PageBy(input)
 87             .ToListAsync();
 88 
 89             var resultListDtos = result<%=paramName%>.MapTo<List<<%=tableName%>Dto>>();
 90             
 91             if (!string.IsNullOrEmpty(input.Sorting)) {
 92                 resultListDtos = resultListDtos.OrderBy(input.Sorting).ToList();
 93             }
 94             
 95             return new PagedResultDto<<%=tableName%>Dto>(
 96             resultCount,
 97             resultListDtos
 98             );
 99         }
100 
101          /// <summary>
102         /// 获取指定条件的数据列表  webapi 无法使用
103         /// </summary>
104         /// <returns>返回数据集合</returns>
105         public async Task<List<<%=tableName%>Dto>> GetListByCodition(Expression<Func<<%=tableName%>, bool>> predicate)
106         {
107 
108             var resultList = await _<%=paramName%>Repository.GetAllListAsync(predicate);
109             return Mapper.Map<List<<%=tableName%>Dto>>(resultList).ToList();
110         }
111 
112 
113          /// <summary>
114         /// 导出excel 具体方法
115         /// </summary>
116         /// <returns>excel文件</returns>
117        /// public async Task<FileDto> Get<%=tableName%>ToExcel()
118         ///{
119         ///    var resultList = await _<%=paramName%>Repository.GetAllListAsync();
120         ///   var <%=paramName%>Dtos= Mapper.Map<List<<%=tableName%>Dto>>(resultList).ToList();
121         ///   return _i<%=tableName%>ListExcelExporter.ExportToFile(<%=paramName%>Dtos);
122        /// }
123 
124         /// <summary>
125         /// 根据指定id 获取数据实体
126         /// </summary>
127         /// <param name="input">当前id</param>
128         /// <returns></returns>
129         public async Task<<%=tableName%>Dto> Get<%=tableName%>ForEditAsync(NullableIdDto<System.Guid> input)
130         {
131             var output = new <%=tableName%>Dto();
132 
133            <%=tableName%>Dto <%=paramName%>EditDto;
134 
135             if (input.Id.HasValue)
136             {
137                 var entity = await _<%=paramName%>Repository.GetAsync(input.Id.Value);
138                 <%=paramName%>EditDto = entity.MapTo<<%=tableName%>Dto>();
139             }
140             else
141             {
142                 <%=paramName%>EditDto = new <%=tableName%>Dto();
143             }
144 
145             output = <%=paramName%>EditDto;
146             return output;
147         }
148 
149         /// <summary>
150         /// 根据Id创建或编辑操作
151         /// </summary>
152         /// <param name="input">实体</param>
153         /// <returns></returns>
154         public async Task CreateOrUpdate<%=tableName%>Async(<%=tableName%>Dto input)
155         {
156             if (!string.IsNullOrWhiteSpace(input.Id))
157             {
158                 await Update(input);
159             }
160             else
161             {
162                 await Create(input);
163             }
164         }
165 
166         /// <summary>
167         /// 新增 
168         /// </summary>
169         /// <param name="input">新增参数</param>
170         /// <returns>新增实体</returns>
171         public async Task<Guid> Create(<%=tableName%>Dto input)
172         {
173             input.Id = new <%=tableName%>().Id.ToString(); 
174             var resultObj = input.MapTo<<%=tableName%>>();
175             var result = await _<%=paramName%>Repository.InsertAsync(resultObj);
176 
177             return result.Id;
178         }
179 
180         /// <summary>
181         /// 修改
182         /// </summary>
183         /// <param name="input">修改参数</param>
184         /// <returns>修改实体</returns>
185         public async Task<<%=tableName%>Dto> Update(<%=tableName%>Dto input)
186         {
187             <%=tableName%> obj = await _<%=paramName%>Repository.GetAsync(new Guid(input.Id));
188             input.MapTo(obj);
189             var result = await _<%=paramName%>Repository.UpdateAsync(obj);
190             return obj.MapTo<<%=tableName%>Dto>();
191         }
192 
193         /// <summary>
194         /// 删除
195         /// </summary>
196         /// <param name="input">删除Dto</param>
197         /// <returns>无返回值</returns>
198         public async System.Threading.Tasks.Task Delete(EntityDto<string> input)
199         {
200             await _<%=paramName%>Repository.DeleteAsync(new Guid(input.Id));
201         }
202 
203         /// <summary>
204         /// 删除 webapi 无法使用
205         /// </summary>
206         /// <param name="predicate">删除条件</param>
207         /// <returns>无返回值</returns>
208         public async System.Threading.Tasks.Task DeleteByCondition(Expression<Func<<%=tableName%>, bool>> predicate)
209         {
210             await _<%=paramName%>Repository.DeleteAsync(predicate);
211           
212         }
213     }
214 
215 
216             <%  } else {%>
217               /// <summary>
218     /// <%=SourceTable.Description%> 业务实现接口
219     /// </summary>
220     public class <%=tableName%>AppService : AbpZeroTemplateAppServiceBase, I<%=tableName%>AppService
221     {
222         private readonly IRepository<<%=tableName%>, <%=tempType%>> _<%=paramName%>Repository;
223         private readonly I<%=tableName%>ListExcelExporter _i<%=tableName%>ListExcelExporter;
224 
225         /// <summary>
226         /// 构造函数自动注入我们所需要的类或接口
227         /// </summary>
228         public <%=tableName%>AppService(IRepository<<%=tableName%>,  <%=tempType%>> <%=paramName%>Repository,I<%=tableName%>ListExcelExporter i<%=tableName%>ListExcelExporter)
229         {
230             _<%=paramName%>Repository = <%=paramName%>Repository;
231             _i<%=tableName%>ListExcelExporter = i<%=tableName%>ListExcelExporter;
232          
233         }
234 
235         /// <summary>
236         /// 获取所有数据列表
237         /// </summary>
238         /// <returns>返回数据集合</returns>
239         public async Task<List<<%=tableName%>Dto>> GetAllList()
240         {
241             //调用Task仓储的特定方法GetAllWithPeople
242             var resultList = await _<%=paramName%>Repository.GetAllListAsync();
243             return Mapper.Map<List<<%=tableName%>Dto>>(resultList).ToList();
244         }
245         
246         /// <summary>
247         /// 获取分页数据列表 分页具体代码需要适当修改，如orderby 需要匹配 创建时间 或者其他数据Id（int）
248         /// </summary>
249         /// <returns>返回数据集合</returns>
250         public async Task<PagedResultDto<<%=tableName%>Dto>> GetPagedListAsync(PagedAndFilteredInputDto input)
251         {
252             var query = _<%=paramName%>Repository.GetAll();
253             //TODO:根据传入的参数添加过滤条件
254 
255             var resultCount = await query.CountAsync();
256             var result<%=paramName%> = await query
257             .OrderBy(x=>x.Id)
258             .PageBy(input)
259             .ToListAsync();
260 
261             var resultListDtos = result<%=paramName%>.MapTo<List<<%=tableName%>Dto>>();
262             return new PagedResultDto<<%=tableName%>Dto>(
263             resultCount,
264             resultListDtos
265             );
266         }
267 
268          /// <summary>
269         /// 获取指定条件的数据列表  webapi 无法使用
270         /// </summary>
271         /// <returns>返回数据集合</returns>
272         public async Task<List<<%=tableName%>Dto>> GetListByCodition(Expression<Func<<%=tableName%>, bool>> predicate)
273         {
274 
275             var resultList = await _<%=paramName%>Repository.GetAllListAsync(predicate);
276             return Mapper.Map<List<<%=tableName%>Dto>>(resultList).ToList();
277         }
278 
279 
280          /// <summary>
281         /// 导出excel 具体方法
282         /// </summary>
283         /// <returns>excel文件</returns>
284        /// public async Task<FileDto> Get<%=tableName%>ToExcel()
285         ///{
286         ///    var resultList = await _<%=paramName%>Repository.GetAllListAsync();
287         ///   var <%=paramName%>Dtos= Mapper.Map<List<<%=tableName%>Dto>>(resultList).ToList();
288         ///   return _i<%=tableName%>ListExcelExporter.ExportToFile(<%=paramName%>Dtos);
289        /// }
290 
291         /// <summary>
292         /// 根据指定id 获取数据实体
293         /// </summary>
294         /// <param name="input">当前id</param>
295         /// <returns></returns>
296         public async Task<<%=tableName%>Dto> Get<%=tableName%>ForEditAsync(NullableIdDto<<%=tempType%>> input)
297         {
298             var output = new <%=tableName%>Dto();
299 
300            <%=tableName%>Dto <%=paramName%>EditDto;
301 
302             if (Convert.ToInt32(input.Id)>0)
303             {
304                 var entity = await _<%=paramName%>Repository.GetAsync(Convert.ToInt32(input.Id));
305                 <%=paramName%>EditDto = entity.MapTo<<%=tableName%>Dto>();
306             }
307             else
308             {
309                 <%=paramName%>EditDto = new <%=tableName%>Dto();
310             }
311 
312             output = <%=paramName%>EditDto;
313             return output;
314         }
315 
316         /// <summary>
317         /// 根据Id创建或编辑操作
318         /// </summary>
319         /// <param name="input">实体</param>
320         /// <returns></returns>
321         public async Task CreateOrUpdate<%=tableName%>Async(<%=tableName%>Dto input)
322         {
323             if (Convert.ToInt32(input.Id)>0)
324             {
325                 await Update(input);
326             }
327             else
328             {
329                 await Create(input);
330             }
331         }
332 
333         /// <summary>
334         /// 新增 
335         /// </summary>
336         /// <param name="input">新增参数</param>
337         /// <returns>新增实体</returns>
338         public async Task<<%=tempType%>> Create(<%=tableName%>Dto input)
339         {
340             input.Id = new <%=tableName%>().Id.ToString(); 
341             var resultObj = input.MapTo<<%=tableName%>>();
342             var result = await _<%=paramName%>Repository.InsertAsync(resultObj);
343 
344             return result.Id;
345         }
346 
347         /// <summary>
348         /// 修改
349         /// </summary>
350         /// <param name="input">修改参数</param>
351         /// <returns>修改实体</returns>
352         public async Task<<%=tableName%>Dto> Update(<%=tableName%>Dto input)
353         {
354             <%=tableName%> obj = await _<%=paramName%>Repository.GetAsync(Convert.ToInt32(input.Id));
355             input.MapTo(obj);
356             var result = await _<%=paramName%>Repository.UpdateAsync(obj);
357             return obj.MapTo<<%=tableName%>Dto>();
358         }
359 
360         /// <summary>
361         /// 删除
362         /// </summary>
363         /// <param name="input">删除Dto</param>
364         /// <returns>无返回值</returns>
365         public async System.Threading.Tasks.Task Delete(EntityDto<string> input)
366         {
367             await _<%=paramName%>Repository.DeleteAsync(Convert.ToInt32(input.Id));
368         }
369 
370         /// <summary>
371         /// 删除 webapi 无法使用
372         /// </summary>
373         /// <param name="predicate">删除条件</param>
374         /// <returns>无返回值</returns>
375         public async System.Threading.Tasks.Task DeleteByCondition(Expression<Func<<%=tableName%>, bool>> predicate)
376         {
377             await _<%=paramName%>Repository.DeleteAsync(predicate);
378           
379         }
380     }
381 
382 
383             <%  }%> 
384       <%  }%> <%  }%>
385 
386    
387 }
388 
389 
390  
391 
392 <script runat="template">
393 <!-- #include file="TemplateUtilities.cs" -->
394 </script>
复制代码
 

 

 

2、AppAuthorizationProvider.cst 是要和AppPermissions.cst 一起使用的，我在实际项目中拆分了原有abp代码，实现了自己的整合的版本，尽量减少对abp 原有代码的耦合

 AbpZeroTemplateApplicationModule 中的Configuration.Authorization.Providers.Add<CustomsAppAuthorizationProvider>(); 确保注入
 

复制代码
<%-- 
Name:
Author: 
Description: 
--%>
<%@ Template Language="C#" TargetLanguage="Text" Src="" Inherits=""Debug="False" CompilerVersion="v4.0" %>
<%@ Assembly Name="SchemaExplorer" %>
<%@ Import Namespace="SchemaExplorer" %>
<%@ Property Name="SourceDatabase" DeepLoad="True" Type="SchemaExplorer.DatabaseSchema" %>
<%@ Property Name="Tables" Type="TableSchemaCollection" Optional="True" Category="2.数据库" Description="Tables to Inclue" %>
<%@ Template Language="C#" TargetLanguage="Text" %>
<%@ Property Name="SampleStringProperty" Default="SomeValue" Type="System.String" %>
<%@ Property Name="SampleBooleanProperty" Default="True" Type="System.Boolean" %>
<%@ Property Name="RootNamespace" Type="String" Default="HashBlockChain" Optional="False" Category="1.名称空间" Description="系统名称空间的根名称." %>
<%@ Property Name="Namespace" Type="String" Default="ZLDB_Domain" Optional="False" Category="1.名称空间" Description="系统当前所属文件夹的名称（命名空间相关）." %>
My static content here.
My dynamic content here: "<%= SampleStringProperty %>"
Call a script method: <%= SampleMethod() %>
<% if (SampleBooleanProperty) { %>
My conditional content here.
<% } %>
<script runat="template">
// My methods here.
public string SampleMethod()
{
  return "Method output.";
}

</script>


using System.Linq;
using Abp.Authorization;
using Abp.Localization;
using <%=RootNamespace%>.Authorization;
//--------------------------------------------------------------------------------------------------------------------------------------------------------------
//简介：Abp 权限配置，生成后要在 AbpZeroTemplateApplicationModule 中的Configuration.Authorization.Providers.Add<CustomsAppAuthorizationProvider>(); 确保注入
//
//
//
//
//作者：
//--------------------------------------------------------------------------------------------------------------------------------------------------------------

namespace <%= RootNamespace %>.<%=Namespace%>.Authorization
{
    /// <summary>
    /// 权限配置都在这里。
    /// 给权限默认设置服务
    /// See <see cref="CustomsAppPermissions"/> for all permission names.
    /// </summary>
    public class CustomsAppAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //在这里配置了自定义 的权限。

            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ?? context.CreatePermission(AppPermissions.Pages, L("Pages"));

            var entityNameModel = pages.Children.FirstOrDefault(p => p.Name == AppPermissions.Pages_Administration)
              ?? pages.CreateChildPermission(AppPermissions.Pages_Administration, L("Administration"));

                 <% foreach(TableSchema t in Tables){ %>
                 <%string tableName=t.Name.ToString(); %>
                 <%string paramName=GetParamName(tableName); %>
                 //<%=t.Description%> 权限
                 var <%=paramName%> = entityNameModel.CreateChildPermission(CustomsAppPermissions.<%=t.Name %>, L("<%=t.Name %>"));
                     <%=paramName%>.CreateChildPermission(CustomsAppPermissions.<%=t.Name %>_Create<%=t.Name %>, L("Create<%=t.Name %>"));
                     <%=paramName%>.CreateChildPermission(CustomsAppPermissions.<%=t.Name %>_Edit<%=t.Name %>, L("Edit<%=t.Name %>"));
                     <%=paramName%>.CreateChildPermission(CustomsAppPermissions.<%=t.Name %>_Delete<%=t.Name %>, L("Delete<%=t.Name %>"));

                <% }%>

        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AbpZeroTemplateConsts.LocalizationSourceName);
        }
    }

}

<script runat="template">
<!-- #include file="TemplateUtilities.cs" -->
</script>
复制代码
 

 3、view 文件夹中的createOrEditModal.cst 

这个是view 视图中基于angular.js，我在这上面封装数据验证，如果不需要的可以自己手动调整，并且 主动拆分为单列和两列的模板，需要自动手动改动

复制代码
<%@ CodeTemplate Language="C#" TargetLanguage="C#" ResponseEncoding="UTF-8" Description="Generates a single entity business class." Debug="True" %>
<%@ Property Name="SourceTable" Type="SchemaExplorer.TableSchema" Category="2.数据库" Description="Database table that this entity should be based on." %>
<%@ Property Name="TablePrefixes" Type="String" Default="" Optional="True" Category="2.数据库" Description="The table prefix to be cut from the class name" %>
<%@ Property Name="RootNamespace" Type="String" Default="ManagementSystem" Optional="False" Category="1.名称空间" Description="系统名称空间的根名称." %>
<%@ Property Name="Title" Type="String" Default="createOrEditModal" Optional="False" Category="1.名称空间" Description="系统名称空间的Model名称." %>
<%@ Property Name="PrefixLength" Type="Int32" Default="0" Optional="False" Category="2.数据库" Description="数据表前缀截取长度." %>
<%@ Assembly Name="SchemaExplorer" %>
<%@ Assembly Name="CodeSmith.BaseTemplates" %>
<%@ Assembly Name="System.Data" %>
<%@ Import Namespace="SchemaExplorer" %>
<%@ Import Namespace="System.Data" %>

<%string tableClass=GetClassName(SourceTable, "", 0); %>
<%string tableName=SourceTable.Name.ToString(); %>
<%string paramName=GetParamName(tableName); %>
<%string tableDescString=GetTableDescriptionName(SourceTable.Description);%>
        

@using Abp.Web.Mvc.Extensions
@using <%=RootNamespace%>.Web.Bundling
@using <%=RootNamespace%>.AbpZeroTemplate
@{
    LocalizationSourceName = AbpZeroTemplateConsts.LocalizationSourceName;
}

@section Styles
{
    @*@Html.IncludeStyle("~/libs/bootstrap-daterangepicker/daterangepicker.css")*@

}

@section Scripts
{

    @*@Html.IncludeScript(ScriptPaths.Angular_DateRangePicker)*@
}

<div>
    @*//mark 1*@
    <form name="<%=paramName%>CreateOrEditForm" role="form" novalidate class="form-validation">
        <div class="modal-header">
            <h4 class="modal-title">
                <span ng-if="vm.<%=paramName%>.id">编辑信息:{{vm.<%=paramName%>.name}}</span>
                <span ng-if="!vm.<%=paramName%>.id">新增信息</span>
            </h4>
        </div>
        <div class="modal-body">
        
        /* 两列模板
        
         <div class="row">
                <div class="col-sm-6">
                
                   //单列的具体代码1
                   
                </div>
                
                 <div class="col-sm-6">
                   
                   //单列的具体代码2
                   
                </div>
           </div>     
        
        */
          <% foreach (ColumnSchema column in SourceTable.Columns) { %> 
          <% if(column.Size>100) {%>
           <div class="form-group form-md-line-input form-md-floating-label no-hint">
                <textarea auto-focus class="form-control" name="<%=GetParamName(column.Name)%>" style="resize: none;" ng-class="{'edited':vm.<%=paramName%>.<%=GetParamName(column.Name)%>}" ng-model="vm.<%=paramName%>.<%=GetParamName(column.Name)%>" required ng-pattern="{填写具体正则表达式}" ng-minlength="10" ng-maxlength="<%=column.Size%>"></textarea>
                <label><%=column.Description%></label>
            </div>
            <div ng-messages="<%=paramName%>CreateOrEditForm.<%=GetParamName(column.Name)%>.$error" ng-show="<%=paramName%>CreateOrEditForm.<%=GetParamName(column.Name)%>.$error">
                <ul class="help-block text-danger">
                    <li ng-show="<%=paramName%>CreateOrEditForm.<%=GetParamName(column.Name)%>.$error.pattern" ng-message="pattern"><%=column.Description%>格式不正确{具体自己再次更改}!</li>
                    <li ng-show="<%=paramName%>CreateOrEditForm.<%=GetParamName(column.Name)%>.$error.required" ng-message="required"><%=column.Description%>不能为空!</li>
                    <li ng-show="<%=paramName%>CreateOrEditForm.<%=GetParamName(column.Name)%>.$error.minlength" ng-message="minlength"><%=column.Description%>最小长度为10!</li>
                    <li ng-show="<%=paramName%>CreateOrEditForm.<%=GetParamName(column.Name)%>.$error.maxlength" ng-message="maxlength"><%=column.Description%>最大长度为<%=column.Size%>!</li>
                </ul>
            </div>
          <%}else {%>
                    <div class="form-group form-md-line-input form-md-floating-label no-hint">
                        <input type="text" class="form-control" name="<%=GetParamName(column.Name)%>" ng-class="{'edited':vm.<%=paramName%>.<%=GetParamName(column.Name)%>}" ng-pattern="{填写具体正则表达式}" ng-model="vm.<%=paramName%>.<%=GetParamName(column.Name)%>" required ng-minlength="10" ng-maxlength="<%=column.Size%>" />
                        <label><%=column.Description%></label>
                    </div>
                    <div ng-messages="<%=paramName%>CreateOrEditForm.<%=GetParamName(column.Name)%>.$error" ng-show="<%=paramName%>CreateOrEditForm.<%=GetParamName(column.Name)%>.$error">
                        <ul class="help-block text-danger">
                            <li ng-show="<%=paramName%>CreateOrEditForm.<%=GetParamName(column.Name)%>.$error.pattern" ng-message="pattern"><%=column.Description%>格式不正确{具体自己再次更改}!</li>
                            <li ng-show="<%=paramName%>CreateOrEditForm.<%=GetParamName(column.Name)%>.$error.required" ng-message="required"><%=column.Description%>不能为空!</li>
                            <li ng-show="<%=paramName%>CreateOrEditForm.<%=GetParamName(column.Name)%>.$error.minlength" ng-message="minlength"><%=column.Description%>最小长度为10!</li>
                            <li ng-show="<%=paramName%>CreateOrEditForm.<%=GetParamName(column.Name)%>.$error.maxlength" ng-message="maxlength"><%=column.Description%>最大长度为<%=column.Size%>!</li>
                        </ul>
                    </div>
          <% }%>
        <% } %>
        </div>
        <div class="modal-footer">
            <button ng-disabled="vm.saving" type="button" class="btn btn-default" ng-click="vm.cancel()">@L("Cancel")</button>
            <button type="submit" button-busy="vm.saving" busy-text="@L("SavingWithThreeDot")" class="btn btn-primary blue" ng-click="vm.save()" ng-disabled="<%=paramName%>CreateOrEditForm.$invalid"><i class="fa fa-save"></i> <span>@L("Save")</span></button>
        </div>
    </form>
</div>
<script runat="template">
<!-- #include file="../TemplateUtilities.cs" -->
</script>
复制代码
 

结果局部展示

 



 

model 实体

复制代码
using System;
using Abp.Authorization.Users;
using Abp.Extensions;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

using Abp;
//----------------------------------------------
//简介：HashBlockChain.ZLDB_Domain  Entity 数据库对应实体
//
//
//
//auther：
//----------------------------------------------
namespace HashBlockChain.ZLDB_Domain
{




    //获取主键Id的命名


    ///链信息表 
    [Table("ChainInfo")]
    public partial class ChainInfo : Entity<int>
    {
        #region Declarations

        /// <summary>
        /// 链名称
        /// </summary>
        [DisplayName("链名称")]
        [StringLength(30)]

        public virtual string ChainName { get; set; }



        /// <summary>
        /// 链Id
        /// </summary>
        [DisplayName("链Id")]
        [StringLength(30)]

        public virtual string ChainId { get; set; }



        /// <summary>
        /// 链描述
        /// </summary>
        [DisplayName("链描述")]
        [StringLength(200)]

        public virtual string ChainDescription { get; set; }



        /// <summary>
        /// 链状态
        /// </summary>

        public virtual int? ChainStatus { get; set; }



        /// <summary>
        /// 排序
        /// </summary>

        public virtual int? Sort { get; set; }



        /// <summary>
        /// 是否可见
        /// </summary>

        public virtual bool? IsEnabled { get; set; }



        /// <summary>
        /// 创建人
        /// </summary>

        public virtual int? CreateUserId { get; set; }



        /// <summary>
        /// 创建时间
        /// </summary>

        public virtual DateTime? CreateDateTime { get; set; }



        /// <summary>
        /// 最后一次修改人
        /// </summary>

        public virtual int? LastEditUserId { get; set; }



        /// <summary>
        /// 最后一次修改时间
        /// </summary>

        public virtual DateTime? LastEditDateTime { get; set; }



        /// <summary>
        /// 节点个数
        /// </summary>

        public virtual int? PeerCount { get; set; }



        /// <summary>
        /// 区块链高度
        /// </summary>

        public virtual long? BlockHeight { get; set; }


        #endregion
    }
}
复制代码
中英文 中文显示

 



 

部分权限生成的代码

 



 

# FluentValidation.Document.Rules
Document FluentValidation rules by interigating a DLL


### Usage 1
```
    using var x = new DocumentRules(OutputType.Markdown);
    x.DocumentAssembly("C:\\MyProject\\MyAssembly.DLL");
```
In `appsettings.json`
``` 
{
  "ExportFolder": {
    "Markdown": "{Desktop}"
  }
}
```
### Usage 2
```
  using var x = new DocumentRules();
  x.DocumentAssembly(OutputType.Markdown);
```
In `appsettings.json`
``` 
{
  "AssemblyToDocument": "C:\\MyProject\\MyAssembly.DLL",

  "ExportFolder": {
    "Markdown": "{Desktop}"
  }
}
```

## Output Location
You can specify the location in `appsettings.json`.

``` 
{
  "ExportFolder": {
    "Markdown": "{Desktop}"
  }
}
```

# Sample output
## Tests.ValidatorsProject.  
----  
### Tests.ValidatorsProject.ProductValidator
| Field | DataType | Model | LINQ Expression | Rule Count | Validator| ValidationValue | Error Message | 
|---|---|---|---|---|---|---|---|  
| AvailableFrom | System.DateTime | Test.ModelsProject.Product | m => m.AvailableFrom | 1 |   GreaterThanOrEqual|01/01/2000 | Available from must be greater than 1 Jan 2000 |
| AvailableTo | Nullable< DateTime> | Test.ModelsProject.Product | m => m.AvailableTo | 1 |   GreaterThanOrEqual| >=  AvailableFrom| Available to must be greater than availble from |
| Name | System.String | Test.ModelsProject.Product | m => m.Name | 2 |   NotEmptyValidator|| Name is requried |
|||||| LengthValidator|3 >= Value <= 5 | Name must be between 3 and 5 characters long. |

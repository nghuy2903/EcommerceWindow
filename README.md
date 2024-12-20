[![Nuget][materialDesignThemes-shield]][materialDesignThemes-url]   [![Nuget][liveChart-shield]][liveChart-url] [![Nuget][entityFramework-shield]][entityFramework-url] [![Nuget][azureStorage-shield]][azureStorage-url]


<!-- MARKDOWN LINKS & IMAGES -->
[materialDesignThemes-shield]: https://img.shields.io/nuget/v/MaterialDesignThemes?label=MaterialDesignThemes&logo=MaterialDesignThemes
[materialDesignThemes-url]: https://www.nuget.org/packages/MaterialDesignThemes/
[entityFramework-shield]: https://img.shields.io/nuget/v/EntityFramework?label=EntityFramework&logo=EntityFramework
[entityFramework-url]: https://www.nuget.org/packages/EntityFramework/
[liveChart-shield]: https://img.shields.io/nuget/v/LiveCharts.Wpf?label=LiveCharts.Wpf&logo=MaterialDesignThemes
[liveChart-url]: https://www.nuget.org/packages/LiveCharts.Wpf
[azureStorage-shield]: https://img.shields.io/badge/Azure%20Storage-Available-brightgreen
[azureStorage-url]: https://www.nuget.org/packages/YourAzurePackageName/

## Hướng dẫn cài đặt
 **1. Tải Project về máy**
 
 **2. Thêm dữ liệu vào database**
 - Tải [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) và [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16)
 - Tạo SQL Server cho máy
*(có thể bỏ qua 2 bước trên nếu đã có SQL Server và SSMS trên máy)*
 - Khởi động phần mềm SQL Server Management Studio (SSMS) và chọn **Server name** muốn lưu dữ liệu
 - Mở file **Database.sql** trong thư mục **WPFEcommerceApp** vừa tải
 - Sau khi nhấn nút **Excute** hoặc **F5** để chạy đoạn code, dữ liệu đã được thêm thành công

**3. Thay đổi ServerName trong SourceCode**
- Mở project vừa tải trong Visual Studio 
- Vào file **App.config** sửa trường **data source** trong **connectionString** thành **Server name** vừa thêm dữ liệu

*Ví dụ:* nếu **Server name** thêm dữ liệu là *DESKTOP*

`<add name="EcommerceAppEntities" connectionString="metadata=res://*/Models.Model.csdl|res://*/Models.Model.ssdl|res://*/Models.Model.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=VU-HOANG\SQLEXPRESS;initial catalog=EcommerceApp;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />`

được sửa lại thành 

`<add name="EcommerceAppEntities" connectionString="metadata=res://*/Models.Model.csdl|res://*/Models.Model.ssdl|res://*/Models.Model.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=DESKTOP;initial catalog=EcommerceApp;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />`
- **Build** project để hoàn tất


# 说明

此存储库用于存储个人类库



## 项目说明

### Himesyo

基本类库，放置一些常用的类。

#### 目录结构

Check：校验相关方法。目前有 MD5 扩展。

Collections：集合，常用数据结构。目前有 可XML序列化的字典、反向队列 等。

ComponentModel：提供一些对说明的反射，主要面向于 PropertyGrid 控件。

Drawing：提供一些和绘制、计算相关的类。包括 扩展Point类型、Math 类型，增加复数及其基本运算和极坐标点等。

Himesyo：位于 Himesyo 命名空间内的最基本的类。

IO：提供与磁盘、文件相关的类。

Linq：对 IEnumerable 扩展。

Logger：日志记录器和与日志相关的类。

Runtime：提供对运行时的操作。例如 反射、附加数据、调用堆栈、异常等。

Win32：Windows API C++ 的封装。

WinForm：依赖于 WinForm 或 WinForm 相关的类。

## 其他存储库

提供对XML注释文档的翻译程序并提供翻译其他文档的接口。

已包含部分文档类型的翻译和部分翻译器。用户可自己扩展文档解析和翻译器。


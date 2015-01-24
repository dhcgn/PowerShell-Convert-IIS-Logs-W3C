@{

# Script module or binary module file associated with this manifest.
RootModule = 'ConvertFromIISLogFile.dll'

# Version number of this module.
ModuleVersion = '1.0.0.12'

# ID used to uniquely identify this module
GUID = '7B2572EC-C63A-4A92-8D8D-102127D2DCB0'

# Author of this module
Author = 'DHCGN'

# Copyright statement for this module
Copyright = '(c) 2015. All rights reserved.'

# Description of the functionality provided by this module
Description = 'Convert log entries from an IIS Log file in W3C format to a friendly format'

# Minimum version of the Windows PowerShell engine required by this module
PowerShellVersion = '3.0'

# Name of the Windows PowerShell host required by this module
# PowerShellHostName = 'ConsoleHost'

# Minimum version of the Windows PowerShell host required by this module
# PowerShellHostVersion = ''

# Minimum version of the .NET Framework required by this module
DotNetFrameworkVersion = '4.0'

# Minimum version of the common language runtime (CLR) required by this module
CLRVersion = '4.0'

# Type files (.ps1xml) to be loaded when importing this module
TypesToProcess = @('ConvertFromIISLogFile.ps1xml')

# Format files (.ps1xml) to be loaded when importing this module
FormatsToProcess = @('ConvertFromIISLogFile.format.ps1xml')

# Modules to import as nested modules of the module specified in RootModule/ModuleToProcess
# NestedModules = @()

# Functions to export from this module
FunctionsToExport = '*'

# Cmdlets to export from this module
CmdletsToExport = '*'

# Aliases to export from this module
AliasesToExport = '*'

# List of all modules packaged with this module.
# ModuleList = @()

# List of all files packaged with this module
# FileList = @()

# Private data to pass to the module specified in RootModule/ModuleToProcess
# PrivateData = ''

# HelpInfo URI of this module
HelpInfoURI = 'https://github.com/dhcgn/PowerShell-Convert-IIS-Logs-W3C-'

}


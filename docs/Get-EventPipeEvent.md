---
external help file: psetw.cmdlets.dll-Help.xml
Module Name: psetwquery
online version:
schema: 2.0.0
---

# Get-EventPipeEvent

## SYNOPSIS
Reads nettrace files, produced by dotnet-trace and outputs events from them as PSObjects.

## SYNTAX

```
Get-EventPipeEvent [-Path <String>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Reads nettrace files, produced by dotnet-trace and outputs events from them as PSObjects.

## EXAMPLES

### Example 1
```powershell
PS C:\> $data = Get-ETWEvents -Path "C:\temp\file.nettrace"
```

Reads events from the file and stores them in the variable

## PARAMETERS

### -Path
Path to a nettrace file

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS

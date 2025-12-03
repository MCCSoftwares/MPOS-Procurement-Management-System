Imports System.Globalization
Imports System.Data
Imports System.Data.SqlClient

' Centralized, culture-agnostic date helpers
Public Module AppDate

    ' Accept both US and PH formats, plus ISO
    Private ReadOnly _dateFormats As String() = {
        "MM/dd/yyyy", "M/d/yyyy",
        "dd/MM/yyyy", "d/M/yyyy",
        "yyyy-MM-dd", "yyyy/M/d", "yyyy/MM/dd"
    }

    ' Parse text the user typed (works on dd/MM or MM/dd machines)
    Public Function ParseDateFlexible(s As String) As DateTime
        If String.IsNullOrWhiteSpace(s) Then
            Throw New FormatException("Date is required.")
        End If

        Dim dt As DateTime
        If DateTime.TryParseExact(s.Trim(), _dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, dt) Then
            Return dt
        End If
        If DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.None, dt) Then
            Return dt
        End If
        Throw New FormatException($"Invalid date: '{s}'. Please use MM/dd/yyyy or dd/MM/yyyy.")
    End Function

    ' Try-parse variant
    Public Function TryParseDateFlexible(s As String, ByRef result As DateTime) As Boolean
        If String.IsNullOrWhiteSpace(s) Then Return False
        If DateTime.TryParseExact(s.Trim(), _dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, result) Then
            Return True
        End If
        Return DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.None, result)
    End Function

    ' Read DateTime? from a SqlDataReader column safely (no locale involved)
    Public Function SafeGetDate(rdr As SqlDataReader, col As String) As DateTime?
        Dim i = rdr.GetOrdinal(col)
        If rdr.IsDBNull(i) Then Return Nothing
        Dim v = rdr.GetValue(i)
        If TypeOf v Is DateTime Then Return DirectCast(v, DateTime)
        ' Handle string/other types defensively
        Dim dt As DateTime
        If DateTime.TryParse(Convert.ToString(v, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, DateTimeStyles.None, dt) Then
            Return dt
        End If
        Return Nothing
    End Function

    ' NEW: read a DateTime? from any object (DataRow, parameters, etc.)
    Public Function FromObj(obj As Object) As DateTime?
        If obj Is Nothing OrElse obj Is DBNull.Value Then Return Nothing
        If TypeOf obj Is DateTime Then Return DirectCast(obj, DateTime)
        Dim s = Convert.ToString(obj, CultureInfo.InvariantCulture)
        Dim dt As DateTime
        If DateTime.TryParseExact(s, _dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, dt) Then Return dt
        If DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, dt) Then Return dt
        Return Nothing
    End Function

    ' Format for UI textboxes consistently
    Public Function ToUiDate(d As DateTime?) As String
        Return If(d.HasValue, d.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), String.Empty)
    End Function

    ' NEW: create a SqlParameter for a DATE column safely
    Public Function AsSqlDateParam(name As String, value As DateTime) As SqlParameter
        Dim p As New SqlParameter(name, SqlDbType.Date)
        p.Value = value.Date
        Return p
    End Function

End Module

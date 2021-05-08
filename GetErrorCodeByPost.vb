Imports System.IO
Imports System.Net
Imports System.Web
Imports System.Xml

Module GetErrorCodeByPost
    Public Function GetCertificate(ByVal url As String, ByVal SOAPAction As String, ByVal requestXml As String) As String
        Dim request As HttpWebRequest = WebRequest.Create(url)
        Dim bytes() As Byte
        bytes = System.Text.Encoding.ASCII.GetBytes(requestXml)
        request.Accept = "text/*"
        request.KeepAlive = True
        request.ContentType = "text/xml; charset=utf-8"
        request.UserAgent = "SLSSoapClient"
        request.Headers.Add("SOAPAction", SOAPAction)
        request.ContentLength = bytes.Length
        request.Method = "POST"
        Dim requestStream As Stream = request.GetRequestStream()
        requestStream.Write(bytes, 0, bytes.Length)
        requestStream.Close()
        Dim response As HttpWebResponse = Nothing
        Dim result = ""
        Try
            response = request.GetResponse()
            If response.StatusCode = HttpStatusCode.OK Then
                Dim responseStream As Stream = response.GetResponseStream()
                result = (New StreamReader(responseStream)).ReadToEnd()
            End If
        Catch ex As WebException
            Dim exMessage As String = ex.Message
            If ex.Response IsNot Nothing Then
                Dim responseReader = New StreamReader(ex.Response.GetResponseStream())
                result = responseReader.ReadToEnd()
            End If
        End Try
        Dim retstring = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(result))
        Try
            Dim soapReader As XmlReader = XmlReader.Create(New StringReader(result))
            soapReader.ReadToFollowing("Value")
            Dim responseXML As String = soapReader.ReadElementContentAsString()
            Return responseXML
        Catch
            Dim soapReader As XmlReader = XmlReader.Create(New StringReader(result))
            soapReader.ReadToFollowing("HRESULT")
            Dim responseXML As String = soapReader.ReadElementContentAsString()
            Return responseXML
        End Try

    End Function
    Public Function GetErrorCode(ByVal url As String, pKeyAlgorithm As String, ByVal requestXml As String) As String
        Dim request As HttpWebRequest = WebRequest.Create(url)
        Dim bytes() As Byte
        bytes = System.Text.Encoding.ASCII.GetBytes(requestXml)
        request.Accept = "text/*"
        request.KeepAlive = True
        request.ContentType = "text/xml; charset=utf-8"
        request.UserAgent = "SLSSoapClient"
        If pKeyAlgorithm.Contains("2005") Then
            request.Headers.Add("SOAPAction", "http://microsoft.com/SL/LicensingService/IssueToken")
        Else
            request.Headers.Add("SOAPAction", "http://microsoft.com/SL/ProductActivationService/IssueToken")
        End If
        request.ContentLength = bytes.Length
        request.Method = "POST"
        Dim requestStream As Stream = request.GetRequestStream()
        requestStream.Write(bytes, 0, bytes.Length)
        requestStream.Close()
        Dim response As HttpWebResponse = Nothing
        Dim result = ""
        Try
            response = request.GetResponse()
            If response.StatusCode = HttpStatusCode.OK Then
                Dim responseStream As Stream = response.GetResponseStream()
                result = (New StreamReader(responseStream)).ReadToEnd()
            End If
        Catch ex As WebException
            Dim exMessage As String = ex.Message
            If ex.Response IsNot Nothing Then
                Dim responseReader = New StreamReader(ex.Response.GetResponseStream())
                result = responseReader.ReadToEnd()
            End If
        End Try
        Dim retstring = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(result))
        Try
            Dim soapReader As XmlReader = XmlReader.Create(New StringReader(result))
            soapReader.ReadToFollowing("Value")
            Dim responseXML As String = soapReader.ReadElementContentAsString()
            Return responseXML
        Catch
            Dim soapReader As XmlReader = XmlReader.Create(New StringReader(result))
            soapReader.ReadToFollowing("HRESULT")
            Dim responseXML As String = soapReader.ReadElementContentAsString()
            Return responseXML
        End Try
    End Function

End Module

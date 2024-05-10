using Microsoft.Reporting.NETCore;
using Newtonsoft.Json;
using System.Data;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Domain.Common;
using Tri_Wall.Domain.DataProviders;

namespace Tri_Wall.Infrastructure.Common.Setting;

public class ReportLayout : IReportLayout
{
    private readonly IDataProviderRepository dataProviderRepository;
    public ReportLayout(IDataProviderRepository dataProviderRepository)
    {
        this.dataProviderRepository = dataProviderRepository;
    }
    public async Task<PrintViewLayoutResponse> CallViewLayout(string Code, string docEntry, string Path)
    {
        var reportSetup = dataProviderRepository.Query(new DataProvider("", "CallLayout", Code)).Result;
        if(reportSetup is null) return await Task.FromResult(new PrintViewLayoutResponse(
            ErrorMessage: "Report not found",
            Data: null,
            ErrCode: ""
            ));
        var type = GetTypeExport(reportSetup.Rows[0]["EXPORTTYPE"].ToString()!);
        LocalReport lr = new LocalReport();
        Stream reportDefinition = File.OpenRead($"{Path}\\Report\\{reportSetup.Rows![0]["FILENAME"]}");
        lr.LoadReportDefinition(reportDefinition);
        foreach (var a in JsonConvert.DeserializeObject<List<ReportBodyResponse>>(reportSetup.Rows?[0]["PROPERTIES"].ToString()!)!)
        {
            DataTable dt =await dataProviderRepository.Query(new DataProvider(StoreName:"",DBType: a.TypeOfParameter, Par1: docEntry));
            lr.DataSources.Add(new ReportDataSource(a.DataSetName.ToString()!, dt));
        }
        lr.Refresh();
        var result = lr.Render(reportSetup.Rows?[0]["EXPORTTYPE"].ToString()!);
        return await Task.FromResult(new PrintViewLayoutResponse(
            ErrCode: "",
            ErrorMessage: "",
            Data: result,
            ApplicationType: type.Item2,
            FileName: type.Item3));
    }
    private Tuple<string, string, string> GetTypeExport(string type)
    {
        if (type == "PDF")
        {
            return Tuple.Create("PDF", "application/pdf", "pdf.pdf");
        }
        else if (type == "WORD")
        {
            return Tuple.Create("WORD", "application/msword", "word.doc");
        }
        else if (type == "EXCEL")
        {
            return Tuple.Create("EXCEL", "application/xlsx", "excel.xls");
        }
        return Tuple.Create("PDF", "application/pdf", "pdf.pdf");
    }
}

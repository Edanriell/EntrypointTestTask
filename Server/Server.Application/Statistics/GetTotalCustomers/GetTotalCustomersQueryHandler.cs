using System.Data;
using System.Text;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;

namespace Server.Application.Statistics.GetTotalCustomers;

internal sealed class GetTotalCustomersQueryHandler : IQueryHandler<GetTotalCustomersQuery, GetTotalCustomersResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetTotalCustomersQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetTotalCustomersResponse>> Handle(
        GetTotalCustomersQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
        var sqlBuilder = new StringBuilder();

        sqlBuilder.Append("""
                              SELECT 
                                  COUNT(*) AS TotalCustomers,
                                  COUNT(*) FILTER (
                                      WHERE u.created_at >= date_trunc('month', CURRENT_DATE - interval '1 month')
                                      AND u.created_at < date_trunc('month', CURRENT_DATE)
                                  ) AS NewThisMonth
                              FROM users u
                          """);

        (int TotalCustomers, int NewThisMonth) users =
            await connection.QuerySingleAsync<(int TotalCustomers, int NewThisMonth)>(sqlBuilder.ToString());

        return new GetTotalCustomersResponse
        {
            TotalCustomers = users.TotalCustomers,
            NewThisMonth = users.NewThisMonth
        };
    }
}

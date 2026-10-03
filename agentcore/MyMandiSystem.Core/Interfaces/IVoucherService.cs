using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface IVoucherService
{
    Task<Voucher?> GetVoucherByIdAsync(int id);
    Task<string> GetNextVoucherNoAsync(VoucherType voucherType, int financialYearId);
    Task SaveVoucherAsync(Voucher voucher);
    Task PostVoucherAsync(int voucherId);
    Task ReverseVoucherAsync(int voucherId, string reason);
}

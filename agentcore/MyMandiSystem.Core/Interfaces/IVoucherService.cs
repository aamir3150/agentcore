using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface IVoucherService
{
    Task<Voucher?> GetVoucherByIdAsync(int id);
    Task<Voucher?> GetVoucherByNoAsync(VoucherType voucherType, string voucherNo, int financialYearId);
    Task<Voucher?> GetPreviousVoucherAsync(VoucherType voucherType, int currentVoucherId, int financialYearId);
    Task<Voucher?> GetNextVoucherAsync(VoucherType voucherType, int currentVoucherId, int financialYearId);
    Task<string> GetNextVoucherNoAsync(VoucherType voucherType, int financialYearId);
    Task SaveVoucherAsync(Voucher voucher);
    Task PostVoucherAsync(int voucherId);
    Task ReverseVoucherAsync(int voucherId, string reason);
    Task DeleteVoucherAsync(int voucherId);
}

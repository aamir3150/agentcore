CREATE view VuAccVouchersAccounts 

 

 As

 

   /* -------------- CREDIT VOUCHERS ---------------- */

 

   select 'CV' as VOUCHERTYPE,CREDITVOUCHERS.VOUCHERNO,CREDITVOUCHERS.VOUCHERDATE,'' as CHEQUENO,null as ChequeDate, null as DepositedDate, Null as ReconcileDate, CREDITVOUCHERSBODY.ACCOUNTNO ,CREDITVOUCHERSBODY.NARRATION, 0 as DEBIT,CREDITVOUCHERSBODY.CREDIT from

 

     CREDITVOUCHERS join CREDITVOUCHERSBODY on CREDITVOUCHERS.VOUCHERNO=CREDITVOUCHERSBODY.VOUCHERNO

 

     and CREDITVOUCHERS.VOUCHERDATE=CREDITVOUCHERSBODY.VOUCHERDATE where CREDITVOUCHERS.ISPOSTED=1 

 

 UNION ALL

 

   select 'CV' as VOUCHERTYPE,CREDITVOUCHERS.VOUCHERNO,CREDITVOUCHERS.VOUCHERDATE,'' as CHEQUENO, null as ChequeDate, null as DepositedDate, Null as ReconcileDate, dbo.RegistryValue('AccCashAccount') as ACCOUNTNO,'Cash received' as NARRATION,

 

     SUM(CREDITVOUCHERSBODY.CREDIT) as DEBIT,0 as CREDIT from

 

     CREDITVOUCHERS join CREDITVOUCHERSBODY on

 

     CREDITVOUCHERS.VOUCHERNO=CREDITVOUCHERSBODY.VOUCHERNO

 

     and CREDITVOUCHERS.VOUCHERDATE=CREDITVOUCHERSBODY.VOUCHERDATE where

 

     CREDITVOUCHERS.ISPOSTED=1

 

     group by CREDITVOUCHERS.VOUCHERNO,CREDITVOUCHERS.VOUCHERDATE

 

 UNION ALL

 

   /* -------------- DEBIT VOUCHERS ---------------- */

 

   select 'DV' as VOUCHERTYPE,DEBITVOUCHERS.VOUCHERNO,DEBITVOUCHERS.VOUCHERDATE,'' as CHEQUENO, null as ChequeDate, null as DepositedDate, Null as ReconcileDate, DEBITVOUCHERSBody.ACCOUNTNO, DEBITVOUCHERSBody.NARRATION, DEBITVOUCHERSBody.DEBIT ,0 as CREDIT from

 

     DEBITVOUCHERSBody join DEBITVOUCHERS on

 

     DEBITVOUCHERS.VOUCHERNO=DEBITVOUCHERSBody.VOUCHERNO

 

     and DEBITVOUCHERS.VOUCHERDATE=DEBITVOUCHERSBody.VOUCHERDATE where

 

     DEBITVOUCHERS.ISPOSTED=1 

 

 UNION ALL

 

   select 'DV' as VOUCHERTYPE,DEBITVOUCHERS.VOUCHERNO,DEBITVOUCHERS.VOUCHERDATE,'' as CHEQUENO, null as ChequeDate, null as DepositedDate, Null as ReconcileDate, dbo.RegistryValue('AccCashAccount') as ACCOUNTNO,'Cash paid' as NARRATION,

 

     0 as DEBIT,

 

     SUM(DEBITVOUCHERSBody.DEBIT) as CREDIT from

 

     DEBITVOUCHERSBody join DEBITVOUCHERS on

 

     DEBITVOUCHERSBody.VOUCHERNO=DEBITVOUCHERS.VOUCHERNO

 

     and DEBITVOUCHERS.VOUCHERDATE=DEBITVOUCHERSBody.VOUCHERDATE where

 

     DEBITVOUCHERS.ISPOSTED=1

 

     group by DEBITVOUCHERS.VOUCHERNO,DEBITVOUCHERS.VOUCHERDATE

 

 UNION ALL

 

   /* -------------- JOURNAL VOUCHERS ---------------- */

 

   select 'JV' as VOUCHERTYPE,JOURNALVOUCHERS.VOUCHERNO,JOURNALVOUCHERS.VOUCHERDATE,'' as CHEQUENO, null as ChequeDate, null as DepositedDate, Null as ReconcileDate, JOURNALVOUCHERSBODY.ACCOUNTNO,JOURNALVOUCHERSBODY.NARRATION,

 

     JOURNALVOUCHERSBODY.DEBIT,JOURNALVOUCHERSBODY.CREDIT from

 

     JOURNALVOUCHERS join JOURNALVOUCHERSBODY on

 

     JOURNALVOUCHERS.VOUCHERNO=JOURNALVOUCHERSBODY.VOUCHERNO

 

     and JOURNALVOUCHERS.VOUCHERDATE=JOURNALVOUCHERSBODY.VOUCHERDATE where

 

     JOURNALVOUCHERS.ISPOSTED=1

 

 UNION ALL

 

   /* -------------- BANK CHEQUES ---------------- */

 

   select 'BC' as VOUCHERTYPE,BANKCHEQUES.VOUCHERNO,BANKCHEQUES.VOUCHERDATE,BANKCHEQUES.CHEQUENO,      ChequeDate, null as DepositedDate, ReconcileDate,

 

     BANKCHEQUES.AccountNo,BANKCHEQUES.NARRATION,BANKCHEQUES.AMOUNT as DEBIT,0 as CREDIT from

 

     BANKCHEQUES where

 

     BANKCHEQUES.ISPOSTED=1 and BANKCHEQUES.ISLOST=0

 

 UNION ALL

 

   select 'BC' as VOUCHERTYPE,BANKCHEQUES.VOUCHERNO,BANKCHEQUES.VOUCHERDATE,BANKCHEQUES.CHEQUENO,      ChequeDate, null as DepositedDate, ReconcileDate,

 

     BANKCHEQUES.BANKACCOUNTNO,BANKCHEQUES.NARRATION,0 as DEBIT,BANKCHEQUES.AMOUNT as CREDIT from

 

     BANKCHEQUES where

 

     BANKCHEQUES.ISPOSTED=1 and BANKCHEQUES.ISLOST=0 

 

 UNION ALL

 

   /* -------------- BANK Deposits ---------------- */

 

   select 'BD' as VOUCHERTYPE,BANKDEPOSIT.VOUCHERNO,BANKDeposit.VOUCHERDATE,BANKDeposit.SlipNo,        ChequeDate, DepositedDate, ReconcileDate,

 

     IsNull(BANKDeposit.AccountNo,'111'), BANKDeposit.NARRATION,0 as Debit,BANKDeposit.AMOUNT as CREDIT from

 

     BANKDeposit where

 

     BANKDeposit.ISPOSTED=1 and BANKDeposit.ISBounced=0 

 

 UNION ALL

 

   select 'BD' as VOUCHERTYPE,BANKDEPOSIT.VOUCHERNO,BANKDeposit.VOUCHERDATE,BANKDeposit.SlipNo,

 

     null as ChequeDate, DepositedDate, ReconcileDate,

 

     BANKDeposit.BankAccountNo,BANKDeposit.NARRATION,BANKDeposit.AMOUNT as DEBIT,0 as CREDIT from

 

     BANKDeposit where

 

     BANKDeposit.ISPOSTED=1 and BANKDeposit.ISBounced=0
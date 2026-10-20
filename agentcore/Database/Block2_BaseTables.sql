MS Sans Serif;

dif exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_BANKCHEQ_REF_1021_CHARTOFA]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[BankCheques] DROP CONSTRAINT FK_BANKCHEQ_REF_1021_CHARTOFA

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_BANKCHEQ_REF_1027_CHARTOFA]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[BankCheques] DROP CONSTRAINT FK_BANKCHEQ_REF_1027_CHARTOFA

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_BANKDEPO_REF_2207_CHARTOFA]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[BankDeposit] DROP CONSTRAINT FK_BANKDEPO_REF_2207_CHARTOFA

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_BANKDEPO_REF_2210_CHARTOFA]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[BankDeposit] DROP CONSTRAINT FK_BANKDEPO_REF_2210_CHARTOFA

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CHARTOFA_REF_3144_CHARTOFA]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[ChartOfAccounts] DROP CONSTRAINT FK_CHARTOFA_REF_3144_CHARTOFA

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CREDITVO_REF_1016_CHARTOFA]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CreditVouchersBody] DROP CONSTRAINT FK_CREDITVO_REF_1016_CHARTOFA

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_DEBITVOU_REF_985_CHARTOFA]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[DebitVouchersBody] DROP CONSTRAINT FK_DEBITVOU_REF_985_CHARTOFA

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_JOURNALV_REF_981_CHARTOFA]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[JournalVouchersBody] DROP CONSTRAINT FK_JOURNALV_REF_981_CHARTOFA

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Parties_ChartOfAccounts]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[Parties] DROP CONSTRAINT FK_Parties_ChartOfAccounts

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_PROMISES_REF_1058_CHARTOFA]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[Promises] DROP CONSTRAINT FK_PROMISES_REF_1058_CHARTOFA

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ProductGroups_Companies]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[ProductGroups] DROP CONSTRAINT FK_ProductGroups_Companies

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Products_Companies]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[Products] DROP CONSTRAINT FK_Products_Companies

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CreditVouchersBody_CreditVouchers]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CreditVouchersBody] DROP CONSTRAINT FK_CreditVouchersBody_CreditVouchers

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CropsPurchaseHeader_CropsPurchaseContracts]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CropsPurchaseHeader] DROP CONSTRAINT FK_CropsPurchaseHeader_CropsPurchaseContracts

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CROPPU_REF_239_CROPPU]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CropsPurchaseBody] DROP CONSTRAINT FK_CROPPU_REF_239_CROPPU

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CropsSaleHeader_CropsSaleContracts]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CropsSaleHeader] DROP CONSTRAINT FK_CropsSaleHeader_CropsSaleContracts

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CROPSALE_REF_281_CROPSALE]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CropsSaleBody] DROP CONSTRAINT FK_CROPSALE_REF_281_CROPSALE

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CropsPurchaseBody_CropsWeighments]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CropsPurchaseBody] DROP CONSTRAINT FK_CropsPurchaseBody_CropsWeighments

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_DebitVouchersBody_DebitVouchers]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[DebitVouchersBody] DROP CONSTRAINT FK_DebitVouchersBody_DebitVouchers

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_JOURNALV_REF_953_JOURNALV]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[JournalVouchersBody] DROP CONSTRAINT FK_JOURNALV_REF_953_JOURNALV

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_JournalVouchersBody_JournalVouchers]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[JournalVouchersBody] DROP CONSTRAINT FK_JournalVouchersBody_JournalVouchers

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CropsPurchaseContracts_Parties]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CropsPurchaseContracts] DROP CONSTRAINT FK_CropsPurchaseContracts_Parties

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CropsPurchaseHeader_Parties]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CropsPurchaseHeader] DROP CONSTRAINT FK_CropsPurchaseHeader_Parties

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CropsSaleContracts_Parties]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CropsSaleContracts] DROP CONSTRAINT FK_CropsSaleContracts_Parties

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CropsSaleHeader_Parties]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CropsSaleHeader] DROP CONSTRAINT FK_CropsSaleHeader_Parties

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CropsWeighments_Parties]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CropsWeighments] DROP CONSTRAINT FK_CropsWeighments_Parties

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Purchase_Parties]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[Purchase] DROP CONSTRAINT FK_Purchase_Parties

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_PurReturns_Parties]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[PurReturns] DROP CONSTRAINT FK_PurReturns_Parties

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_SaleReturns_Parties]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[SaleReturns] DROP CONSTRAINT FK_SaleReturns_Parties

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Sales_Parties]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[Sales] DROP CONSTRAINT FK_Sales_Parties

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Products_ProductGroups]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[Products] DROP CONSTRAINT FK_Products_ProductGroups

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CropsSaleBody_Products]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CropsSaleBody] DROP CONSTRAINT FK_CropsSaleBody_Products

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CropsWeighments_Products]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CropsWeighments] DROP CONSTRAINT FK_CropsWeighments_Products

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_OpeningStock_Products]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[OpeningStock] DROP CONSTRAINT FK_OpeningStock_Products

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_PurchaseBody_Products]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[PurchaseBody] DROP CONSTRAINT FK_PurchaseBody_Products

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_PurchaseReturnBody_Products1]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[PurReturnsBody] DROP CONSTRAINT FK_PurchaseReturnBody_Products1

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_SaleReturnsBody_Products]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[SaleReturnsBody] DROP CONSTRAINT FK_SaleReturnsBody_Products

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_SalesBody_Products]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[SalesBody] DROP CONSTRAINT FK_SalesBody_Products

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_USERSTAS_REF_1064_PROJECT_]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[UsersTasks] DROP CONSTRAINT FK_USERSTAS_REF_1064_PROJECT_

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_PurchaseReturnBody_PurchaseReturn]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[PurReturnsBody] DROP CONSTRAINT FK_PurchaseReturnBody_PurchaseReturn

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_PurchaseBody_Purchase]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[PurchaseBody] DROP CONSTRAINT FK_PurchaseBody_Purchase

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_PurchaseReturn_Purchase]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[PurReturns] DROP CONSTRAINT FK_PurchaseReturn_Purchase

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_SaleReturnsBody_SaleReturns]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[SaleReturnsBody] DROP CONSTRAINT FK_SaleReturnsBody_SaleReturns

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_SaleReturns_Sales]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[SaleReturns] DROP CONSTRAINT FK_SaleReturns_Sales

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_SalesBody_Sales]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[SalesBody] DROP CONSTRAINT FK_SalesBody_Sales

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Parties_Sectors]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[Parties] DROP CONSTRAINT FK_Parties_Sectors

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_SaleReturns_Sectors]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[SaleReturns] DROP CONSTRAINT FK_SaleReturns_Sectors

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Sales_Sectors]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[Sales] DROP CONSTRAINT FK_Sales_Sectors

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Sectors_Towns]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[Sectors] DROP CONSTRAINT FK_Sectors_Towns

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Products_Units]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[Products] DROP CONSTRAINT FK_Products_Units

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_BANKCHEQ_REF_1009_USERS]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[BankCheques] DROP CONSTRAINT FK_BANKCHEQ_REF_1009_USERS

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_BANKDEPO_REF_2204_USERS]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[BankDeposit] DROP CONSTRAINT FK_BANKDEPO_REF_2204_USERS

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CHARTOFA_REF_1039_USERS]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[ChartOfAccounts] DROP CONSTRAINT FK_CHARTOFA_REF_1039_USERS

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CREDITVO_REF_1002_USERS]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CreditVouchers] DROP CONSTRAINT FK_CREDITVO_REF_1002_USERS

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CROPSPURCONT_REFERENCE_USERS]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CropsPurchaseContracts] DROP CONSTRAINT FK_CROPSPURCONT_REFERENCE_USERS

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CROPPURHEAD_USERID_USERS]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CropsPurchaseHeader] DROP CONSTRAINT FK_CROPPURHEAD_USERID_USERS

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CROPSSALECONT_USERID_USERS]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CropsSaleContracts] DROP CONSTRAINT FK_CROPSSALECONT_USERID_USERS

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CROPSALEHEAD_USERID_USERS]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CropsSaleHeader] DROP CONSTRAINT FK_CROPSALEHEAD_USERID_USERS

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_CROPSWE_REFERENCE_USERS]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[CropsWeighments] DROP CONSTRAINT FK_CROPSWE_REFERENCE_USERS

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_DEBITVOU_REF_989_USERS]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[DebitVouchers] DROP CONSTRAINT FK_DEBITVOU_REF_989_USERS

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_JOURNALV_REF_976_USERS]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[JournalVouchers] DROP CONSTRAINT FK_JOURNALV_REF_976_USERS

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_PROMISES_REF_1035_USERS]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[Promises] DROP CONSTRAINT FK_PROMISES_REF_1035_USERS

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_TASKSREP_REFERENCE_USERS]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[TasksRepeated] DROP CONSTRAINT FK_TASKSREP_REFERENCE_USERS

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_TASKSTOD_REFERENCE_USERS]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[TasksToDo] DROP CONSTRAINT FK_TASKSTOD_REFERENCE_USERS

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_USERSTAS_REF_1067_USERS]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)

ALTER TABLE [dbo].[UsersTasks] DROP CONSTRAINT FK_USERSTAS_REF_1067_USERS

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[tu_creditvouchers]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[tu_creditvouchers]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[td_creditvouchers]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[td_creditvouchers]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ti_creditvouchersbody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[ti_creditvouchersbody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[tu_creditvouchersbody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[tu_creditvouchersbody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[td_creditvouchersbody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[td_creditvouchersbody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[tu_debitvouchers]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[tu_debitvouchers]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[td_debitvouchers]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[td_debitvouchers]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ti_debitvouchersbody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[ti_debitvouchersbody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[tu_debitvouchersbody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[tu_debitvouchersbody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[td_debitvouchersbody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[td_debitvouchersbody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[tu_journalvouchers]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[tu_journalvouchers]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[td_journalvouchers]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[td_journalvouchers]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ti_journalvouchersbody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[ti_journalvouchersbody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[tu_journalvouchersbody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[tu_journalvouchersbody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[td_journalvouchersbody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[td_journalvouchersbody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[trg_delete_OpeningStock]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[trg_delete_OpeningStock]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[trg_insert_OpeningStock]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[trg_insert_OpeningStock]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[trg_update_OpeningStock]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[trg_update_OpeningStock]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ti_promises]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[ti_promises]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[tu_promises]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[tu_promises]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[trg_insert_PurReturnsBody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[trg_insert_PurReturnsBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[trg_update_PurReturnsBody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[trg_update_PurReturnsBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[trg_delete_PurReturnsBody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[trg_delete_PurReturnsBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[trg_insert_PurchaseBody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[trg_insert_PurchaseBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[trg_update_PurchaseBody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[trg_update_PurchaseBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[trg_delete_PurchaseBody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[trg_delete_PurchaseBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[trg_insert_SaleReturnsBody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[trg_insert_SaleReturnsBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[trg_update_SaleReturnsBody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[trg_update_SaleReturnsBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[trg_delete_SaleReturnsBody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[trg_delete_SaleReturnsBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[trg_insert_SalesBody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[trg_insert_SalesBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[trg_update_SalesBody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[trg_update_SalesBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[trg_delete_SalesBody]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)

drop trigger [dbo].[trg_delete_SalesBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AccountsBalances]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[AccountsBalances]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[BankCheques]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[BankCheques]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[BankDeposit]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[BankDeposit]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ChartOfAccounts]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[ChartOfAccounts]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Companies]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[Companies]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CreditVouchers]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[CreditVouchers]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CreditVouchersBody]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[CreditVouchersBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CropsPurchaseBody]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[CropsPurchaseBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CropsPurchaseContracts]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[CropsPurchaseContracts]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CropsPurchaseHeader]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[CropsPurchaseHeader]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CropsSaleBody]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[CropsSaleBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CropsSaleContracts]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[CropsSaleContracts]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CropsSaleHeader]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[CropsSaleHeader]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CropsWeighments]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[CropsWeighments]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CurrentStock]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[CurrentStock]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DebitVouchers]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[DebitVouchers]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DebitVouchersBody]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[DebitVouchersBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[JournalVouchers]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[JournalVouchers]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[JournalVouchersBody]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[JournalVouchersBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[OpeningStock]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[OpeningStock]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Parties]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[Parties]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ProductGroups]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[ProductGroups]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Products]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[Products]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Project_Registry]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[Project_Registry]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Project_Tasks]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[Project_Tasks]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Promises]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[Promises]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurReturns]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[PurReturns]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurReturnsBody]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[PurReturnsBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Purchase]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[Purchase]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseBody]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[PurchaseBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SaleReturns]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[SaleReturns]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SaleReturnsBody]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[SaleReturnsBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Sales]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[Sales]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SalesBody]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[SalesBody]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Sectors]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[Sectors]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[TasksRepeated]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[TasksRepeated]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[TasksToDo]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[TasksToDo]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Towns]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[Towns]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Units]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[Units]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Users]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[Users]

GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UsersTasks]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)

drop table [dbo].[UsersTasks]

GO



CREATE TABLE [dbo].[AccountsBalances] (

    [AccountNo] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [Debit] [decimal](14, 2) NULL ,

    [Credit] [decimal](14, 2) NULL ,

    [Bal] [decimal](14, 2) NULL ,

    [BalType] [varchar] (2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[BankCheques] (

    [VoucherNo] [numeric](10, 0) NOT NULL ,

    [VoucherDate] [smalldatetime] NOT NULL ,

    [ChequeNo] [varchar] (15) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [ChequeDate] [smalldatetime] NULL ,

    [ReconcileDate] [smalldatetime] NULL ,

    [BankAccountNo] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [AccountNo] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [UserNo] [smallint] NOT NULL ,

    [Amount] [decimal](11, 2) NOT NULL ,

    [ReceivedBy] [varchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [Narration] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [IsLost] [bit] NULL ,

    [IsPosted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[BankDeposit] (

    [VoucherNo] [numeric](10, 0) NOT NULL ,

    [VoucherDate] [smalldatetime] NOT NULL ,

    [IsCheque] [bit] NOT NULL ,

    [ChequeNo] [varchar] (15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [ChequeDate] [smalldatetime] NULL ,

    [SlipNo] [varchar] (15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [DepositedDate] [smalldatetime] NULL ,

    [ReconcileDate] [smalldatetime] NULL ,

    [BankAccountNo] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [AccountNo] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [UserNo] [smallint] NOT NULL ,

    [Amount] [decimal](11, 2) NOT NULL ,

    [DepositedBy] [varchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [Narration] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [IsBounced] [bit] NOT NULL ,

    [IsPosted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[ChartOfAccounts] (

    [AccountNo] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [UserID] [smallint] NOT NULL ,

    [AccountName] [varchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [AccountType] [varchar] (11) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [AccountDepth] [numeric](3, 0) NULL ,

    [Narration] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [ParentAccountNo] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [OpeningDebit] [decimal](13, 2) NOT NULL ,

    [OpeningCredit] [decimal](13, 2) NOT NULL ,

    [AdjustedDebit] [decimal](13, 2) NOT NULL ,

    [AdjustedCredit] [decimal](13, 2) NOT NULL ,

    [IsDetailed] [bit] NOT NULL ,

    [IsLocked] [bit] NOT NULL ,

    [IsPosted] [bit] NOT NULL ,

    [IsEditable] [bit] NOT NULL ,

    [BalFlag] [bit] NOT NULL ,

    [PLFlag] [varchar] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [ExpFlag] [bit] NOT NULL ,

    [ExpenseLimit] [decimal](13, 2) NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[Companies] (

    [CompanyID] [varchar] (2) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [CompanyName] [varchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[CreditVouchers] (

    [VoucherNo] [numeric](10, 0) NOT NULL ,

    [VoucherDate] [smalldatetime] NOT NULL ,

    [UserNo] [smallint] NOT NULL ,

    [IsPosted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[CreditVouchersBody] (

    [SerialNo] [numeric](10, 0) IDENTITY (1, 1) NOT NULL ,

    [AccountNo] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [VoucherNo] [numeric](10, 0) NOT NULL ,

    [VoucherDate] [smalldatetime] NOT NULL ,

    [Narration] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [Credit] [decimal](11, 2) NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[CropsPurchaseBody] (

    [SERIALNO] [numeric](10, 0) IDENTITY (1, 1) NOT NULL ,

    [CropPurchaseID] [int] NOT NULL ,

    [WeighmentID] [int] NOT NULL ,

    [Weight] [decimal](11, 4) NOT NULL ,

    [Price] [decimal](11, 2) NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[CropsPurchaseContracts] (

    [PurContractID] [int] NOT NULL ,

    [ContractDate] [smalldatetime] NOT NULL ,

    [ValidUpTo] [smalldatetime] NOT NULL ,

    [VendorID] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [SupplyMode] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [SupplyModeQty] [smallint] NULL ,

    [Weight] [decimal](11, 4) NOT NULL ,

    [Price] [decimal](11, 2) NOT NULL ,

    [ExpPlus] [decimal](6, 2) NOT NULL ,

    [ExpMinus] [decimal](6, 2) NOT NULL ,

    [ComRatio] [decimal](4, 2) NOT NULL ,

    [UserID] [smallint] NOT NULL ,

    [IsPosted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[CropsPurchaseHeader] (

    [CropPurchaseID] [int] NOT NULL ,

    [PurchaseDate] [smalldatetime] NOT NULL ,

    [PurContractID] [int] NULL ,

    [VendorID] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [ComRatio] [decimal](4, 2) NOT NULL ,

    [ExpPlus] [decimal](11, 4) NOT NULL ,

    [ExpMinus] [decimal](11, 4) NOT NULL ,

    [UserID] [smallint] NOT NULL ,

    [Remarks] [varchar] (75) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [IsPosted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[CropsSaleBody] (

    [SERIALNO] [int] IDENTITY (1, 1) NOT NULL ,

    [CropSaleID] [int] NOT NULL ,

    [ProductID] [varchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [Multiplier] [decimal](7, 3) NOT NULL ,

    [SupplyMode] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [SupplyModeQty] [smallint] NULL ,

    [Deduction] [decimal](11, 4) NOT NULL ,

    [Weight] [decimal](11, 4) NOT NULL ,

    [Price] [decimal](11, 2) NOT NULL ,

    [IsDeleted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[CropsSaleContracts] (

    [CropSaleContractID] [int] NOT NULL ,

    [ContractDate] [smalldatetime] NOT NULL ,

    [ValidUpTo] [smalldatetime] NOT NULL ,

    [CustomerID] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [SupplyMode] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [SupplyModeQty] [smallint] NULL ,

    [Price] [decimal](11, 2) NOT NULL ,

    [ExpPlus] [decimal](6, 2) NOT NULL ,

    [ExpMinus] [decimal](6, 2) NOT NULL ,

    [ComRatio] [decimal](4, 2) NOT NULL ,

    [UserID] [smallint] NOT NULL ,

    [IsPosted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[CropsSaleHeader] (

    [CropSaleID] [int] NOT NULL ,

    [SaleDate] [smalldatetime] NOT NULL ,

    [CropSaleContractID] [int] NULL ,

    [CustomerID] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [STRatio] [decimal](4, 2) NOT NULL ,

    [WHTRatio] [decimal](4, 2) NOT NULL ,

    [ExpPlus] [decimal](6, 2) NOT NULL ,

    [ExpMinus] [decimal](6, 2) NOT NULL ,

    [ComRatio] [decimal](4, 2) NOT NULL ,

    [UserID] [smallint] NOT NULL ,

    [Remarks] [varchar] (75) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [IsPosted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[CropsWeighments] (

    [WeighmentID] [int] NOT NULL ,

    [WeighmentDate] [smalldatetime] NOT NULL ,

    [VendorID] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [ProductID] [varchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [Multiplier] [decimal](7, 3) NOT NULL ,

    [SupplyMode] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [SupplyModeQty] [smallint] NULL ,

    [InitialGrossWeight] [decimal](11, 4) NOT NULL ,

    [TareWeight] [decimal](8, 2) NOT NULL ,

    [GrossWeight] [decimal](11, 4) NOT NULL ,

    [Deduction] [decimal](6, 2) NOT NULL ,

    [Shortage] [decimal](7, 2) NOT NULL ,

    [UserID] [smallint] NOT NULL ,

    [Remarks] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [IsFinalized] [bit] NOT NULL ,

    [IsPosted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[CurrentStock] (

    [ProductID] [varchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [Qty] [decimal](11, 2) NOT NULL ,

    [Cost] [decimal](18, 6) NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[DebitVouchers] (

    [VoucherNo] [numeric](10, 0) NOT NULL ,

    [VoucherDate] [smalldatetime] NOT NULL ,

    [UserNo] [smallint] NOT NULL ,

    [IsPosted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[DebitVouchersBody] (

    [SerialNo] [numeric](10, 0) IDENTITY (1, 1) NOT NULL ,

    [AccountNo] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [VoucherNo] [numeric](10, 0) NOT NULL ,

    [VoucherDate] [smalldatetime] NOT NULL ,

    [Narration] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [Debit] [decimal](11, 2) NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[JournalVouchers] (

    [VoucherNo] [numeric](10, 0) NOT NULL ,

    [VoucherDate] [smalldatetime] NOT NULL ,

    [UserNo] [smallint] NOT NULL ,

    [IsPosted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[JournalVouchersBody] (

    [SerialNo] [numeric](10, 0) IDENTITY (1, 1) NOT NULL ,

    [AccountNo] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [VoucherNo] [numeric](10, 0) NOT NULL ,

    [VoucherDate] [smalldatetime] NOT NULL ,

    [Narration] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [Debit] [decimal](11, 2) NOT NULL ,

    [Credit] [decimal](11, 2) NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[OpeningStock] (

    [ProductID] [varchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [Multiplier] [decimal](7, 3) NOT NULL ,

    [Qty] [decimal](7, 2) NOT NULL ,

    [Bonus] [decimal](7, 2) NOT NULL ,

    [Price] [decimal](11, 2) NOT NULL ,

    [DiscRatio] [decimal](4, 2) NOT NULL ,

    [DiscValue] [decimal](7, 2) NOT NULL ,

    [STRatio] [decimal](4, 2) NOT NULL ,

    [STValue] [decimal](7, 2) NOT NULL ,

    [TTLValue] [decimal](9, 2) NOT NULL ,

    [TTLSalesTax] [decimal](9, 2) NOT NULL ,

    [Cost] [decimal](18, 6) NOT NULL ,

    [IsDeleted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[Parties] (

    [PartyID] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [PartyName] [varchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [ContactPerson] [varchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [SectorID] [smallint] NULL ,

    [Address] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [City] [varchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [Phone1] [varchar] (15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [Phone2] [varchar] (15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [Mobile] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [Fax] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [EMail] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [IsCustomer] [bit] NOT NULL ,

    [IsVendor] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[ProductGroups] (

    [CompanyID] [varchar] (2) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [GroupID] [tinyint] NOT NULL ,

    [GroupName] [varchar] (25) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[Products] (

    [ProductID] [varchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [CompanyID] [varchar] (2) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [GroupID] [tinyint] NULL ,

    [ProductName] [varchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [UnitID] [tinyint] NOT NULL ,

    [PurchasePrice] [decimal](9, 2) NOT NULL ,

    [SalePrice] [decimal](9, 2) NOT NULL ,

    [PurDiscRatio] [decimal](4, 2) NOT NULL ,

    [SaleDiscRatio] [decimal](4, 2) NOT NULL ,

    [PurDiscValue] [decimal](6, 2) NOT NULL ,

    [SaleDiscValue] [decimal](6, 2) NOT NULL ,

    [PurSTRatio] [decimal](4, 2) NOT NULL ,

    [SaleSTRatio] [decimal](4, 2) NOT NULL ,

    [IsCrop] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[Project_Registry] (

    [RegistryKey] [varchar] (40) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [ParentRegistryKey] [varchar] (40) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [Narration] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [Value] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [IsEditable] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[Project_Tasks] (

    [TaskKey] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [TaskName] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [TaskGroup] [varchar] (60) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [IsAutoPost] [bit] NOT NULL ,

    [IsPosting] [bit] NOT NULL ,

    [Edition] [varchar] (3) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[Promises] (

    [PromiseNo] [numeric](10, 0) NOT NULL ,

    [IsPayment] [bit] NOT NULL ,

    [CurrentDate] [smalldatetime] NOT NULL ,

    [PromiseDate] [smalldatetime] NOT NULL ,

    [Narration] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [Amount] [decimal](11, 2) NOT NULL ,

    [PartyNo] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [UserNo] [smallint] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[PurReturns] (

    [PurReturnID] [int] NOT NULL ,

    [PurchaseID] [int] NULL ,

    [PurReturnDate] [smalldatetime] NOT NULL ,

    [VendorID] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [ExpPlus] [decimal](6, 2) NOT NULL ,

    [ExpMinus] [decimal](6, 2) NOT NULL ,

    [ComRatio] [decimal](4, 2) NOT NULL ,

    [ReceivedAmount] [decimal](9, 2) NOT NULL ,

    [Narration] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [IsPosted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[PurReturnsBody] (

    [PurReturnID] [int] NOT NULL ,

    [ProductID] [varchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [Multiplier] [decimal](7, 3) NOT NULL ,

    [Qty] [decimal](7, 2) NOT NULL ,

    [Bonus] [decimal](7, 2) NOT NULL ,

    [Price] [decimal](11, 2) NOT NULL ,

    [DiscRatio] [decimal](4, 2) NOT NULL ,

    [DiscValue] [decimal](7, 2) NOT NULL ,

    [STRatio] [decimal](4, 2) NOT NULL ,

    [STValue] [decimal](7, 2) NOT NULL ,

    [TTLValue] [decimal](11, 2) NOT NULL ,

    [TTLSalesTax] [decimal](11, 2) NOT NULL ,

    [Cost] [decimal](18, 6) NOT NULL ,

    [IsDeleted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[Purchase] (

    [PurchaseID] [int] NOT NULL ,

    [PurchaseDate] [smalldatetime] NOT NULL ,

    [VendorID] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [BillNo] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [BillDate] [smalldatetime] NULL ,

    [ExpPlus] [decimal](6, 2) NOT NULL ,

    [ExpMinus] [decimal](6, 2) NOT NULL ,

    [ComRatio] [decimal](4, 2) NOT NULL ,

    [PaidAmount] [decimal](9, 2) NOT NULL ,

    [Narration] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [IsPosted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[PurchaseBody] (

    [PurchaseID] [int] NOT NULL ,

    [ProductID] [varchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [Multiplier] [decimal](7, 3) NOT NULL ,

    [Qty] [decimal](7, 2) NOT NULL ,

    [Bonus] [decimal](7, 2) NOT NULL ,

    [Price] [decimal](11, 2) NOT NULL ,

    [DiscRatio] [decimal](4, 2) NOT NULL ,

    [DiscValue] [decimal](7, 2) NOT NULL ,

    [STRatio] [decimal](4, 2) NOT NULL ,

    [STValue] [decimal](7, 2) NOT NULL ,

    [TTLValue] [decimal](11, 2) NOT NULL ,

    [TTLSalesTax] [decimal](11, 2) NOT NULL ,

    [Cost] [decimal](18, 6) NOT NULL ,

    [IsDeleted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[SaleReturns] (

    [SaleReturnID] [int] NOT NULL ,

    [SaleReturnDate] [smalldatetime] NOT NULL ,

    [SaleID] [int] NULL ,

    [CustomerID] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [SectorID] [smallint] NOT NULL ,

    [ExpPlus] [decimal](6, 2) NOT NULL ,

    [ExpMinus] [decimal](6, 2) NOT NULL ,

    [ComRatio] [decimal](4, 2) NOT NULL ,

    [PaidAmount] [decimal](9, 2) NOT NULL ,

    [SpecialDiscount] [decimal](9, 2) NOT NULL ,

    [Narration] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [IsPosted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[SaleReturnsBody] (

    [SaleReturnID] [int] NOT NULL ,

    [ProductID] [varchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [Multiplier] [decimal](7, 3) NOT NULL ,

    [Qty] [decimal](7, 2) NOT NULL ,

    [Bonus] [decimal](7, 2) NOT NULL ,

    [Price] [decimal](11, 2) NOT NULL ,

    [DiscRatio] [decimal](4, 2) NOT NULL ,

    [DiscValue] [decimal](7, 2) NOT NULL ,

    [STRatio] [decimal](4, 2) NOT NULL ,

    [STValue] [decimal](7, 2) NOT NULL ,

    [TTLValue] [decimal](11, 2) NOT NULL ,

    [TTLSalesTax] [decimal](11, 2) NOT NULL ,

    [Cost] [decimal](18, 6) NOT NULL ,

    [IsDeleted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[Sales] (

    [SaleID] [int] NOT NULL ,

    [SaleDate] [smalldatetime] NOT NULL ,

    [CustomerID] [varchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [SectorID] [smallint] NOT NULL ,

    [ExpPlus] [decimal](6, 2) NOT NULL ,

    [ExpMinus] [decimal](6, 2) NOT NULL ,

    [ComRatio] [decimal](4, 2) NOT NULL ,

    [ReceivedAmount] [decimal](9, 2) NOT NULL ,

    [SpecialDiscount] [decimal](9, 2) NOT NULL ,

    [Narration] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [IsPosted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[SalesBody] (

    [SaleID] [int] NOT NULL ,

    [ProductID] [varchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [Multiplier] [decimal](7, 3) NOT NULL ,

    [Qty] [decimal](7, 2) NOT NULL ,

    [Bonus] [decimal](7, 2) NOT NULL ,

    [Price] [decimal](11, 2) NOT NULL ,

    [DiscRatio] [decimal](4, 2) NOT NULL ,

    [DiscValue] [decimal](7, 2) NOT NULL ,

    [STRatio] [decimal](4, 2) NOT NULL ,

    [STValue] [decimal](7, 2) NOT NULL ,

    [TTLValue] [decimal](11, 2) NOT NULL ,

    [TTLSalesTax] [decimal](11, 2) NOT NULL ,

    [Cost] [decimal](18, 6) NOT NULL ,

    [IsDeleted] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[Sectors] (

    [SectorID] [smallint] NOT NULL ,

    [TownID] [int] NOT NULL ,

    [SectorName] [varchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[TasksRepeated] (

    [RepeatedTaskId] [int] NOT NULL ,

    [UserNo] [smallint] NOT NULL ,

    [Subject] [varchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [Description] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [Priority] [varchar] (6) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [RemindTime] [smalldatetime] NULL ,

    [RepeatPattern] [varchar] (7) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [DailyGap] [tinyint] NULL ,

    [WeekGap] [tinyint] NULL ,

    [Monday] [bit] NULL ,

    [Tuesday] [bit] NULL ,

    [Wednesday] [bit] NULL ,

    [Thursday] [bit] NULL ,

    [Friday] [bit] NULL ,

    [Saturday] [bit] NULL ,

    [Sunday] [bit] NULL ,

    [MonthGap] [tinyint] NULL ,

    [MonthDay] [tinyint] NULL ,

    [YearDate] [smalldatetime] NULL ,

    [StartingDate] [smalldatetime] NULL ,

    [EndingDate] [smalldatetime] NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[TasksToDo] (

    [SubjectId] [int] NOT NULL ,

    [UserNo] [smallint] NOT NULL ,

    [Subject] [varchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [Description] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [DueDateTime] [smalldatetime] NULL ,

    [Priority] [varchar] (6) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [Status] [varchar] (25) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [RemindDateTime] [smalldatetime] NULL ,

    [Sound] [bit] NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[Towns] (

    [TownID] [int] NOT NULL ,

    [TownName] [varchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[Units] (

    [UnitID] [tinyint] NOT NULL ,

    [UnitName] [varchar] (15) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [Multiplier] [decimal](7, 3) NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[Users] (

    [UserID] [smallint] NOT NULL ,

    [UserName] [varchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [Password] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,

    [Narration] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,

    [IsAdministrator] [bit] NOT NULL ,

    [IsEnabled] [bit] NOT NULL 

) ON [PRIMARY]

GO



CREATE TABLE [dbo].[UsersTasks] (

    [UserID] [smallint] NOT NULL ,

    [TaskKey] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 

) ON [PRIMARY]

GO



ALTER TABLE [dbo].[BankCheques] WITH NOCHECK ADD 

    CONSTRAINT [PK_BANKCHEQUES] PRIMARY KEY  CLUSTERED 

    (

        [VoucherNo],

        [VoucherDate]

    )  ON [PRIMARY] ,

    CONSTRAINT [AK_BC_CHECKNO_BANKCHEQ] UNIQUE  NONCLUSTERED 

    (

        [BankAccountNo],

        [ChequeNo]

    )  ON [PRIMARY] ,

    CONSTRAINT [CKC_AMOUNT_BANKCHEQ] CHECK ([Amount] > 0),

    CONSTRAINT [CKC_VOUCHERNO_BANKCHEQ] CHECK ([VoucherNo] > 0),

    CONSTRAINT [CKT_BANKCHEQUES] CHECK (([ReconcileDate] is null or [ChequeDate] is null or [ReconcileDate] >= [ChequeDate]) and ([IsPosted] = 0 or ([IsLost] = 1 or [ReconcileDate] is not null) and ([IsLost] = 0 or [ReconcileDate] is null)))

GO



ALTER TABLE [dbo].[BankDeposit] WITH NOCHECK ADD 

    CONSTRAINT [PK_BANKDEPOSIT] PRIMARY KEY  CLUSTERED 

    (

        [VoucherNo],

        [VoucherDate]

    )  ON [PRIMARY] ,

    CONSTRAINT [AK_BD_SLIPNO_BANKDEPO] UNIQUE  NONCLUSTERED 

    (

        [BankAccountNo],

        [SlipNo],

        [ChequeNo]

    )  ON [PRIMARY] ,

    CONSTRAINT [CKT_BANKDEPOSIT] CHECK (([ReconcileDate] is null or [ReconcileDate] >= [DepositedDate]) and ([DepositedDate] is null or [DepositedDate] >= [ChequeDate]) and ([IsCheque] = 0 and [AccountNo] is null and [ChequeNo] is null and [ChequeDate] is null or [IsCheque] = 1 and [AccountNo] is not null and [ChequeNo] is not null and [ChequeDate] is not null) and ([IsPosted] = 0 or ([IsBounced] = 1 or [ReconcileDate] is not null) and ([IsBounced] = 0 or [ReconcileDate] is null)))

GO



ALTER TABLE [dbo].[ChartOfAccounts] WITH NOCHECK ADD 

    CONSTRAINT [DF__ChartOfAc__Openi__239E4DCF] DEFAULT (0) FOR [OpeningDebit],

    CONSTRAINT [DF__ChartOfAc__Openi__25869641] DEFAULT (0) FOR [OpeningCredit],

    CONSTRAINT [DF__ChartOfAc__Adjus__276EDEB3] DEFAULT (0) FOR [AdjustedDebit],

    CONSTRAINT [DF__ChartOfAc__Adjus__29572725] DEFAULT (0) FOR [AdjustedCredit],

    CONSTRAINT [DF__ChartOfAc__IsLoc__2B3F6F97] DEFAULT (0) FOR [IsLocked],

    CONSTRAINT [DF__ChartOfAc__IsPos__2C3393D0] DEFAULT (0) FOR [IsPosted],

    CONSTRAINT [DF__ChartOfAc__IsEdi__2D27B809] DEFAULT (1) FOR [IsEditable],

    CONSTRAINT [DF__ChartOfAc__BalFl__2E1BDC42] DEFAULT (0) FOR [BalFlag],

    CONSTRAINT [DF__ChartOfAc__PLFla__2F10007B] DEFAULT (' ') FOR [PLFlag],

    CONSTRAINT [DF__ChartOfAc__ExpFl__30F848ED] DEFAULT (0) FOR [ExpFlag],

    CONSTRAINT [PK_CHARTOFACCOUNTS] PRIMARY KEY  CLUSTERED 

    (

        [AccountNo]

    )  ON [PRIMARY] ,

    CONSTRAINT [CKC_ADJUSTEDCREDIT_CHARTOFA] CHECK ([AdjustedCredit] >= 0),

    CONSTRAINT [CKC_ADJUSTEDDEBIT_CHARTOFA] CHECK ([AdjustedDebit] >= 0),

    CONSTRAINT [CKC_ChartOfAccount_ACCOUNTTYPE] CHECK ([AccountType] = 'Party' or ([AccountType] = 'Expense' or ([AccountType] = 'Revenue' or ([AccountType] = 'Capital' or ([AccountType] = 'Liability' or [AccountType] = 'Asset'))))),

    CONSTRAINT [CKC_OPENINGCREDIT_CHARTOFA] CHECK ([OpeningCredit] >= 0),

    CONSTRAINT [CKC_OPENINGDEBIT_CHARTOFA] CHECK ([OpeningDebit] >= 0),

    CONSTRAINT [CKC_PLFLAG_CHARTOFA] CHECK ([PLFlag] = '-' or ([PLFlag] = '+' or [PLFlag] = ' ')),

    CONSTRAINT [CKT_CHARTOFACCOUNTS] CHECK (([OpeningCredit] >= 0 or [OpeningDebit] >= 0) and ([OpeningCredit] = 0 or [OpeningDebit] = 0) and (convert(numeric(10),[AccountNo]) >= 1 and convert(numeric(10),[AccountNo]) <= 6 or [ParentAccountNo] is not null) and ([ParentAccountNo] is null or left([AccountNo],len([ParentAccountNo])) = [ParentAccountNo]) and ([IsDetailed] = 1 or [OpeningCredit] = 0 and [OpeningDebit] = 0 and [AdjustedCredit] = 0 and [AdjustedDebit] = 0))

GO



ALTER TABLE [dbo].[Companies] WITH NOCHECK ADD 

    CONSTRAINT [PK_Companies] PRIMARY KEY  CLUSTERED 

    (

        [CompanyID]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[CreditVouchers] WITH NOCHECK ADD 

    CONSTRAINT [CreditVouchersGeneral] PRIMARY KEY  CLUSTERED 

    (

        [VoucherNo],

        [VoucherDate]

    )  ON [PRIMARY] ,

    CONSTRAINT [CKC_VOUCHERNO_CREDITVO] CHECK ([VoucherNo] > 0)

GO



ALTER TABLE [dbo].[CreditVouchersBody] WITH NOCHECK ADD 

    CONSTRAINT [CreditVouchersDetailed] PRIMARY KEY  CLUSTERED 

    (

        [SerialNo]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[CropsPurchaseBody] WITH NOCHECK ADD 

    CONSTRAINT [PK_CROPSPURCHASEBODY] PRIMARY KEY  CLUSTERED 

    (

        [SERIALNO]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[CropsPurchaseContracts] WITH NOCHECK ADD 

    CONSTRAINT [PK_CROPSPURCHASECONTRACTS] PRIMARY KEY  CLUSTERED 

    (

        [PurContractID]

    )  ON [PRIMARY] ,

    CONSTRAINT [CKC_PRICE_CROPPURCONT] CHECK ([Price] >= 0.01),

    CONSTRAINT [CKC_WEIGHT_CROPPURCONT] CHECK ([Weight] >= 1),

    CONSTRAINT [CKT_CROPPURCHASECONTRACTS] CHECK ([VALIDUPTO] >= [CONTRACTDATE])

GO



ALTER TABLE [dbo].[CropsPurchaseHeader] WITH NOCHECK ADD 

    CONSTRAINT [PK_CROPPURCHASEHEADER] PRIMARY KEY  CLUSTERED 

    (

        [CropPurchaseID]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[CropsSaleBody] WITH NOCHECK ADD 

    CONSTRAINT [PK_CROPSSALEBODY] PRIMARY KEY  CLUSTERED 

    (

        [SERIALNO]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[CropsSaleContracts] WITH NOCHECK ADD 

    CONSTRAINT [PK_CROPSSALECONTRACTS] PRIMARY KEY  CLUSTERED 

    (

        [CropSaleContractID]

    )  ON [PRIMARY] ,

    CONSTRAINT [CKT_CROPSSALECONTRACTS] CHECK ([VALIDUPTO] >= [CONTRACTDATE])

GO



ALTER TABLE [dbo].[CropsSaleHeader] WITH NOCHECK ADD 

    CONSTRAINT [PK_CROPSSALEHEADER] PRIMARY KEY  CLUSTERED 

    (

        [CropSaleID]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[CropsWeighments] WITH NOCHECK ADD 

    CONSTRAINT [PK_CROPSWEIGHMENTS] PRIMARY KEY  CLUSTERED 

    (

        [WeighmentID]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[CurrentStock] WITH NOCHECK ADD 

    CONSTRAINT [PK_CurrentStock] PRIMARY KEY  CLUSTERED 

    (

        [ProductID]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[DebitVouchers] WITH NOCHECK ADD 

    CONSTRAINT [DebitVouchersGeneral] PRIMARY KEY  CLUSTERED 

    (

        [VoucherNo],

        [VoucherDate]

    )  ON [PRIMARY] ,

    CONSTRAINT [CKC_VOUCHERNO_DEBITVOU] CHECK ([VoucherNo] > 0)

GO



ALTER TABLE [dbo].[DebitVouchersBody] WITH NOCHECK ADD 

    CONSTRAINT [DebitVouchersDetailed] PRIMARY KEY  CLUSTERED 

    (

        [SerialNo]

    )  ON [PRIMARY] ,

    CONSTRAINT [CKC_VOUCHERNO_DEBITVOUCHER] CHECK ([VoucherNo] >= 1)

GO



ALTER TABLE [dbo].[JournalVouchers] WITH NOCHECK ADD 

    CONSTRAINT [JournalVouchersGeneral] PRIMARY KEY  CLUSTERED 

    (

        [VoucherNo],

        [VoucherDate]

    )  ON [PRIMARY] ,

    CONSTRAINT [CKC_VOUCHERNO_JOURNALV] CHECK ([VoucherNo] > 0)

GO



ALTER TABLE [dbo].[JournalVouchersBody] WITH NOCHECK ADD 

    CONSTRAINT [JournalVouchersDetailed] PRIMARY KEY  CLUSTERED 

    (

        [SerialNo]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[OpeningStock] WITH NOCHECK ADD 

    CONSTRAINT [PK_OpeningStock] PRIMARY KEY  CLUSTERED 

    (

        [ProductID]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[Parties] WITH NOCHECK ADD 

    CONSTRAINT [PK_Parties] PRIMARY KEY  CLUSTERED 

    (

        [PartyID]

    )  ON [PRIMARY] ,

    CONSTRAINT [CK_PartyType] CHECK ([IsVendor] = 1 or [IsCustomer] = 1)

GO



ALTER TABLE [dbo].[ProductGroups] WITH NOCHECK ADD 

    CONSTRAINT [PK_ProductGroups] PRIMARY KEY  CLUSTERED 

    (

        [CompanyID],

        [GroupID]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[Products] WITH NOCHECK ADD 

    CONSTRAINT [DF_Products_PurDiscRatio] DEFAULT (0) FOR [PurDiscRatio],

    CONSTRAINT [DF_Products_SaleDiscRatio] DEFAULT (0) FOR [SaleDiscRatio],

    CONSTRAINT [DF_Products_PurDiscValue] DEFAULT (0) FOR [PurDiscValue],

    CONSTRAINT [DF_Products_SaleDiscValue] DEFAULT (0) FOR [SaleDiscValue],

    CONSTRAINT [DF_Products_PurSTRatio] DEFAULT (0) FOR [PurSTRatio],

    CONSTRAINT [DF_Products_SaleSTRatio] DEFAULT (0) FOR [SaleSTRatio],

    CONSTRAINT [PK_Products] PRIMARY KEY  NONCLUSTERED 

    (

        [ProductID]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[Project_Registry] WITH NOCHECK ADD 

    CONSTRAINT [DF__Project_R__IsEdi__7B905C75] DEFAULT (0) FOR [IsEditable],

    CONSTRAINT [Registry] PRIMARY KEY  CLUSTERED 

    (

        [RegistryKey]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[Project_Tasks] WITH NOCHECK ADD 

    CONSTRAINT [DF__Project_T__TaskG__7E6CC920] DEFAULT ('') FOR [TaskGroup],

    CONSTRAINT [Tasks] PRIMARY KEY  CLUSTERED 

    (

        [TaskKey]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[Promises] WITH NOCHECK ADD 

    CONSTRAINT [DF__Promises__Curren__4AB81AF0] DEFAULT (getdate()) FOR [CurrentDate],

    CONSTRAINT [DF__Promises__Amount__4BAC3F29] DEFAULT (0) FOR [Amount],

    CONSTRAINT [PostDatedVouchers] PRIMARY KEY  CLUSTERED 

    (

        [PromiseNo]

    )  ON [PRIMARY] ,

    CONSTRAINT [CKT_PROMISES] CHECK ([PromiseDate] > [CurrentDate])

GO



ALTER TABLE [dbo].[PurReturns] WITH NOCHECK ADD 

    CONSTRAINT [PK_PurchaseReturn] PRIMARY KEY  CLUSTERED 

    (

        [PurReturnID]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[PurReturnsBody] WITH NOCHECK ADD 

    CONSTRAINT [PK_PurchaseReturnBody] PRIMARY KEY  CLUSTERED 

    (

        [PurReturnID],

        [ProductID]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[Purchase] WITH NOCHECK ADD 

    CONSTRAINT [PK_Purchase] PRIMARY KEY  CLUSTERED 

    (

        [PurchaseID]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[PurchaseBody] WITH NOCHECK ADD 

    CONSTRAINT [PK_PurchaseBody] PRIMARY KEY  CLUSTERED 

    (

        [PurchaseID],

        [ProductID]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[SaleReturns] WITH NOCHECK ADD 

    CONSTRAINT [PK_SaleReturns] PRIMARY KEY  CLUSTERED 

    (

        [SaleReturnID]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[SaleReturnsBody] WITH NOCHECK ADD 

    CONSTRAINT [PK_SaleReturnsBody] PRIMARY KEY  CLUSTERED 

    (

        [SaleReturnID],

        [ProductID]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[Sales] WITH NOCHECK ADD 

    CONSTRAINT [PK_Sales] PRIMARY KEY  CLUSTERED 

    (

        [SaleID]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[SalesBody] WITH NOCHECK ADD 

    CONSTRAINT [PK_SalesBody] PRIMARY KEY  CLUSTERED 

    (

        [SaleID],

        [ProductID]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[Sectors] WITH NOCHECK ADD 

    CONSTRAINT [PK_Sectors] PRIMARY KEY  CLUSTERED 

    (

        [SectorID]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[TasksRepeated] WITH NOCHECK ADD 

    CONSTRAINT [DF__TasksRepe__Prior__5812160E] DEFAULT ('Normal') FOR [Priority],

    CONSTRAINT [PK_TASKSREPEATED] PRIMARY KEY  CLUSTERED 

    (

        [RepeatedTaskId]

    )  ON [PRIMARY] ,

    CONSTRAINT [CKC_PRIORITY_TASKSREP] CHECK ([Priority] is null or ([Priority] = 'Low' or ([Priority] = 'High' or [Priority] = 'Normal'))),

    CONSTRAINT [CKC_REPEATPATTERN_TASKSREP] CHECK ([RepeatPattern] is null or ([RepeatPattern] = 'Yearly' or ([RepeatPattern] = 'Monthly' or ([RepeatPattern] = 'Weekly' or [RepeatPattern] = 'Daily'))))

GO



ALTER TABLE [dbo].[TasksToDo] WITH NOCHECK ADD 

    CONSTRAINT [DF__TasksToDo__Sound__5EBF139D] DEFAULT (1) FOR [Sound],

    CONSTRAINT [PK_TASKSTODO] PRIMARY KEY  CLUSTERED 

    (

        [SubjectId]

    )  ON [PRIMARY] ,

    CONSTRAINT [CKC_PRIORITY_TASKSTOD] CHECK ([Priority] is null or ([Priority] = 'Low' or ([Priority] = 'High' or [Priority] = 'Normal'))),

    CONSTRAINT [CKC_STATUS_TASKSTOD] CHECK ([Status] is null or ([Status] = 'Done' or ([Status] = 'In progress' or ([Status] = 'Waiting on someone else' or [Status] = 'Not started'))))

GO



ALTER TABLE [dbo].[Towns] WITH NOCHECK ADD 

    CONSTRAINT [PK_Towns] PRIMARY KEY  CLUSTERED 

    (

        [TownID]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[Units] WITH NOCHECK ADD 

    CONSTRAINT [PK_Units] PRIMARY KEY  CLUSTERED 

    (

        [UnitID]

    )  ON [PRIMARY] ,

    CONSTRAINT [CK_Units_Multiplier] CHECK ([Multiplier] >= 1)

GO



ALTER TABLE [dbo].[Users] WITH NOCHECK ADD 

    CONSTRAINT [DF__Users__IsEnabled__060DEAE8] DEFAULT (1) FOR [IsEnabled],

    CONSTRAINT [PK_Users] PRIMARY KEY  CLUSTERED 

    (

        [UserID]

    )  ON [PRIMARY] ,

    CONSTRAINT [AK_USERNAME_USERS] UNIQUE  NONCLUSTERED 

    (

        [UserName]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[UsersTasks] WITH NOCHECK ADD 

    CONSTRAINT [AK_UK1_USERSTAS] UNIQUE  NONCLUSTERED 

    (

        [UserID],

        [TaskKey]

    )  ON [PRIMARY] 

GO



ALTER TABLE [dbo].[BankCheques] ADD 

    CONSTRAINT [FK_BANKCHEQ_REF_1009_USERS] FOREIGN KEY 

    (

        [UserNo]

    ) REFERENCES [dbo].[Users] (

        [UserID]

    ),

    CONSTRAINT [FK_BANKCHEQ_REF_1021_CHARTOFA] FOREIGN KEY 

    (

        [BankAccountNo]

    ) REFERENCES [dbo].[ChartOfAccounts] (

        [AccountNo]

    ),

    CONSTRAINT [FK_BANKCHEQ_REF_1027_CHARTOFA] FOREIGN KEY 

    (

        [AccountNo]

    ) REFERENCES [dbo].[ChartOfAccounts] (

        [AccountNo]

    )

GO



ALTER TABLE [dbo].[BankDeposit] ADD 

    CONSTRAINT [FK_BANKDEPO_REF_2204_USERS] FOREIGN KEY 

    (

        [UserNo]

    ) REFERENCES [dbo].[Users] (

        [UserID]

    ),

    CONSTRAINT [FK_BANKDEPO_REF_2207_CHARTOFA] FOREIGN KEY 

    (

        [AccountNo]

    ) REFERENCES [dbo].[ChartOfAccounts] (

        [AccountNo]

    ),

    CONSTRAINT [FK_BANKDEPO_REF_2210_CHARTOFA] FOREIGN KEY 

    (

        [BankAccountNo]

    ) REFERENCES [dbo].[ChartOfAccounts] (

        [AccountNo]

    )

GO



ALTER TABLE [dbo].[ChartOfAccounts] ADD 

    CONSTRAINT [FK_CHARTOFA_REF_1039_USERS] FOREIGN KEY 

    (

        [UserID]

    ) REFERENCES [dbo].[Users] (

        [UserID]

    ),

    CONSTRAINT [FK_CHARTOFA_REF_3144_CHARTOFA] FOREIGN KEY 

    (

        [ParentAccountNo]

    ) REFERENCES [dbo].[ChartOfAccounts] (

        [AccountNo]

    )

GO



ALTER TABLE [dbo].[CreditVouchers] ADD 

    CONSTRAINT [FK_CREDITVO_REF_1002_USERS] FOREIGN KEY 

    (

        [UserNo]

    ) REFERENCES [dbo].[Users] (

        [UserID]

    )

GO



ALTER TABLE [dbo].[CreditVouchersBody] ADD 

    CONSTRAINT [FK_CREDITVO_REF_1016_CHARTOFA] FOREIGN KEY 

    (

        [AccountNo]

    ) REFERENCES [dbo].[ChartOfAccounts] (

        [AccountNo]

    ),

    CONSTRAINT [FK_CreditVouchersBody_CreditVouchers] FOREIGN KEY 

    (

        [VoucherNo],

        [VoucherDate]

    ) REFERENCES [dbo].[CreditVouchers] (

        [VoucherNo],

        [VoucherDate]

    ) ON DELETE CASCADE 

GO



ALTER TABLE [dbo].[CropsPurchaseBody] ADD 

    CONSTRAINT [FK_CROPPU_REF_239_CROPPU] FOREIGN KEY 

    (

        [CropPurchaseID]

    ) REFERENCES [dbo].[CropsPurchaseHeader] (

        [CropPurchaseID]

    ) ON DELETE CASCADE ,

    CONSTRAINT [FK_CropsPurchaseBody_CropsWeighments] FOREIGN KEY 

    (

        [WeighmentID]

    ) REFERENCES [dbo].[CropsWeighments] (

        [WeighmentID]

    )

GO



ALTER TABLE [dbo].[CropsPurchaseContracts] ADD 

    CONSTRAINT [FK_CropsPurchaseContracts_Parties] FOREIGN KEY 

    (

        [VendorID]

    ) REFERENCES [dbo].[Parties] (

        [PartyID]

    ),

    CONSTRAINT [FK_CROPSPURCONT_REFERENCE_USERS] FOREIGN KEY 

    (

        [UserID]

    ) REFERENCES [dbo].[Users] (

        [UserID]

    )

GO



ALTER TABLE [dbo].[CropsPurchaseHeader] ADD 

    CONSTRAINT [FK_CROPPURHEAD_USERID_USERS] FOREIGN KEY 

    (

        [UserID]

    ) REFERENCES [dbo].[Users] (

        [UserID]

    ),

    CONSTRAINT [FK_CropsPurchaseHeader_CropsPurchaseContracts] FOREIGN KEY 

    (

        [PurContractID]

    ) REFERENCES [dbo].[CropsPurchaseContracts] (

        [PurContractID]

    ),

    CONSTRAINT [FK_CropsPurchaseHeader_Parties] FOREIGN KEY 

    (

        [VendorID]

    ) REFERENCES [dbo].[Parties] (

        [PartyID]

    )

GO



ALTER TABLE [dbo].[CropsSaleBody] ADD 

    CONSTRAINT [FK_CROPSALE_REF_281_CROPSALE] FOREIGN KEY 

    (

        [CropSaleID]

    ) REFERENCES [dbo].[CropsSaleHeader] (

        [CropSaleID]

    ) ON DELETE CASCADE ,

    CONSTRAINT [FK_CropsSaleBody_Products] FOREIGN KEY 

    (

        [ProductID]

    ) REFERENCES [dbo].[Products] (

        [ProductID]

    )

GO



ALTER TABLE [dbo].[CropsSaleContracts] ADD 

    CONSTRAINT [FK_CROPSSALECONT_USERID_USERS] FOREIGN KEY 

    (

        [UserID]

    ) REFERENCES [dbo].[Users] (

        [UserID]

    ),

    CONSTRAINT [FK_CropsSaleContracts_Parties] FOREIGN KEY 

    (

        [CustomerID]

    ) REFERENCES [dbo].[Parties] (

        [PartyID]

    )

GO



ALTER TABLE [dbo].[CropsSaleHeader] ADD 

    CONSTRAINT [FK_CROPSALEHEAD_USERID_USERS] FOREIGN KEY 

    (

        [UserID]

    ) REFERENCES [dbo].[Users] (

        [UserID]

    ),

    CONSTRAINT [FK_CropsSaleHeader_CropsSaleContracts] FOREIGN KEY 

    (

        [CropSaleContractID]

    ) REFERENCES [dbo].[CropsSaleContracts] (

        [CropSaleContractID]

    ),

    CONSTRAINT [FK_CropsSaleHeader_Parties] FOREIGN KEY 

    (

        [CustomerID]

    ) REFERENCES [dbo].[Parties] (

        [PartyID]

    )

GO



ALTER TABLE [dbo].[CropsWeighments] ADD 

    CONSTRAINT [FK_CROPSWE_REFERENCE_USERS] FOREIGN KEY 

    (

        [UserID]

    ) REFERENCES [dbo].[Users] (

        [UserID]

    ),

    CONSTRAINT [FK_CropsWeighments_Parties] FOREIGN KEY 

    (

        [VendorID]

    ) REFERENCES [dbo].[Parties] (

        [PartyID]

    ),

    CONSTRAINT [FK_CropsWeighments_Products] FOREIGN KEY 

    (

        [ProductID]

    ) REFERENCES [dbo].[Products] (

        [ProductID]

    )

GO



ALTER TABLE [dbo].[DebitVouchers] ADD 

    CONSTRAINT [FK_DEBITVOU_REF_989_USERS] FOREIGN KEY 

    (

        [UserNo]

    ) REFERENCES [dbo].[Users] (

        [UserID]

    )

GO



ALTER TABLE [dbo].[DebitVouchersBody] ADD 

    CONSTRAINT [FK_DEBITVOU_REF_985_CHARTOFA] FOREIGN KEY 

    (

        [AccountNo]

    ) REFERENCES [dbo].[ChartOfAccounts] (

        [AccountNo]

    ),

    CONSTRAINT [FK_DebitVouchersBody_DebitVouchers] FOREIGN KEY 

    (

        [VoucherNo],

        [VoucherDate]

    ) REFERENCES [dbo].[DebitVouchers] (

        [VoucherNo],

        [VoucherDate]

    ) ON DELETE CASCADE 

GO



ALTER TABLE [dbo].[JournalVouchers] ADD 

    CONSTRAINT [FK_JOURNALV_REF_976_USERS] FOREIGN KEY 

    (

        [UserNo]

    ) REFERENCES [dbo].[Users] (

        [UserID]

    )

GO



ALTER TABLE [dbo].[JournalVouchersBody] ADD 

    CONSTRAINT [FK_JOURNALV_REF_953_JOURNALV] FOREIGN KEY 

    (

        [VoucherNo],

        [VoucherDate]

    ) REFERENCES [dbo].[JournalVouchers] (

        [VoucherNo],

        [VoucherDate]

    ),

    CONSTRAINT [FK_JOURNALV_REF_981_CHARTOFA] FOREIGN KEY 

    (

        [AccountNo]

    ) REFERENCES [dbo].[ChartOfAccounts] (

        [AccountNo]

    ),

    CONSTRAINT [FK_JournalVouchersBody_JournalVouchers] FOREIGN KEY 

    (

        [VoucherNo],

        [VoucherDate]

    ) REFERENCES [dbo].[JournalVouchers] (

        [VoucherNo],

        [VoucherDate]

    ) ON DELETE CASCADE 

GO



ALTER TABLE [dbo].[OpeningStock] ADD 

    CONSTRAINT [FK_OpeningStock_Products] FOREIGN KEY 

    (

        [ProductID]

    ) REFERENCES [dbo].[Products] (

        [ProductID]

    )

GO



ALTER TABLE [dbo].[Parties] ADD 

    CONSTRAINT [FK_Parties_ChartOfAccounts] FOREIGN KEY 

    (

        [PartyID]

    ) REFERENCES [dbo].[ChartOfAccounts] (

        [AccountNo]

    ),

    CONSTRAINT [FK_Parties_Sectors] FOREIGN KEY 

    (

        [SectorID]

    ) REFERENCES [dbo].[Sectors] (

        [SectorID]

    )

GO



ALTER TABLE [dbo].[ProductGroups] ADD 

    CONSTRAINT [FK_ProductGroups_Companies] FOREIGN KEY 

    (

        [CompanyID]

    ) REFERENCES [dbo].[Companies] (

        [CompanyID]

    )

GO



ALTER TABLE [dbo].[Products] ADD 

    CONSTRAINT [FK_Products_Companies] FOREIGN KEY 

    (

        [CompanyID]

    ) REFERENCES [dbo].[Companies] (

        [CompanyID]

    ),

    CONSTRAINT [FK_Products_ProductGroups] FOREIGN KEY 

    (

        [CompanyID],

        [GroupID]

    ) REFERENCES [dbo].[ProductGroups] (

        [CompanyID],

        [GroupID]

    ),

    CONSTRAINT [FK_Products_Units] FOREIGN KEY 

    (

        [UnitID]

    ) REFERENCES [dbo].[Units] (

        [UnitID]

    )

GO



ALTER TABLE [dbo].[Promises] ADD 

    CONSTRAINT [FK_PROMISES_REF_1035_USERS] FOREIGN KEY 

    (

        [UserNo]

    ) REFERENCES [dbo].[Users] (

        [UserID]

    ),

    CONSTRAINT [FK_PROMISES_REF_1058_CHARTOFA] FOREIGN KEY 

    (

        [PartyNo]

    ) REFERENCES [dbo].[ChartOfAccounts] (

        [AccountNo]

    )

GO



ALTER TABLE [dbo].[PurReturns] ADD 

    CONSTRAINT [FK_PurchaseReturn_Purchase] FOREIGN KEY 

    (

        [PurchaseID]

    ) REFERENCES [dbo].[Purchase] (

        [PurchaseID]

    ),

    CONSTRAINT [FK_PurReturns_Parties] FOREIGN KEY 

    (

        [VendorID]

    ) REFERENCES [dbo].[Parties] (

        [PartyID]

    )

GO



ALTER TABLE [dbo].[PurReturnsBody] ADD 

    CONSTRAINT [FK_PurchaseReturnBody_Products1] FOREIGN KEY 

    (

        [ProductID]

    ) REFERENCES [dbo].[Products] (

        [ProductID]

    ),

    CONSTRAINT [FK_PurchaseReturnBody_PurchaseReturn] FOREIGN KEY 

    (

        [PurReturnID]

    ) REFERENCES [dbo].[PurReturns] (

        [PurReturnID]

    )

GO



ALTER TABLE [dbo].[Purchase] ADD 

    CONSTRAINT [FK_Purchase_Parties] FOREIGN KEY 

    (

        [VendorID]

    ) REFERENCES [dbo].[Parties] (

        [PartyID]

    )

GO



ALTER TABLE [dbo].[PurchaseBody] ADD 

    CONSTRAINT [FK_PurchaseBody_Products] FOREIGN KEY 

    (

        [ProductID]

    ) REFERENCES [dbo].[Products] (

        [ProductID]

    ),

    CONSTRAINT [FK_PurchaseBody_Purchase] FOREIGN KEY 

    (

        [PurchaseID]

    ) REFERENCES [dbo].[Purchase] (

        [PurchaseID]

    )

GO



ALTER TABLE [dbo].[SaleReturns] ADD 

    CONSTRAINT [FK_SaleReturns_Parties] FOREIGN KEY 

    (

        [CustomerID]

    ) REFERENCES [dbo].[Parties] (

        [PartyID]

    ),

    CONSTRAINT [FK_SaleReturns_Sales] FOREIGN KEY 

    (

        [SaleID]

    ) REFERENCES [dbo].[Sales] (

        [SaleID]

    ),

    CONSTRAINT [FK_SaleReturns_Sectors] FOREIGN KEY 

    (

        [SectorID]

    ) REFERENCES [dbo].[Sectors] (

        [SectorID]

    )

GO



ALTER TABLE [dbo].[SaleReturnsBody] ADD 

    CONSTRAINT [FK_SaleReturnsBody_Products] FOREIGN KEY 

    (

        [ProductID]

    ) REFERENCES [dbo].[Products] (

        [ProductID]

    ),

    CONSTRAINT [FK_SaleReturnsBody_SaleReturns] FOREIGN KEY 

    (

        [SaleReturnID]

    ) REFERENCES [dbo].[SaleReturns] (

        [SaleReturnID]

    )

GO



ALTER TABLE [dbo].[Sales] ADD 

    CONSTRAINT [FK_Sales_Parties] FOREIGN KEY 

    (

        [CustomerID]

    ) REFERENCES [dbo].[Parties] (

        [PartyID]

    ),

    CONSTRAINT [FK_Sales_Sectors] FOREIGN KEY 

    (

        [SectorID]

    ) REFERENCES [dbo].[Sectors] (

        [SectorID]

    )

GO



ALTER TABLE [dbo].[SalesBody] ADD 

    CONSTRAINT [FK_SalesBody_Products] FOREIGN KEY 

    (

        [ProductID]

    ) REFERENCES [dbo].[Products] (

        [ProductID]

    ),

    CONSTRAINT [FK_SalesBody_Sales] FOREIGN KEY 

    (

        [SaleID]

    ) REFERENCES [dbo].[Sales] (

        [SaleID]

    )

GO



ALTER TABLE [dbo].[Sectors] ADD 

    CONSTRAINT [FK_Sectors_Towns] FOREIGN KEY 

    (

        [TownID]

    ) REFERENCES [dbo].[Towns] (

        [TownID]

    )

GO



ALTER TABLE [dbo].[TasksRepeated] ADD 

    CONSTRAINT [FK_TASKSREP_REFERENCE_USERS] FOREIGN KEY 

    (

        [UserNo]

    ) REFERENCES [dbo].[Users] (

        [UserID]

    ) ON DELETE CASCADE 

GO



ALTER TABLE [dbo].[TasksToDo] ADD 

    CONSTRAINT [FK_TASKSTOD_REFERENCE_USERS] FOREIGN KEY 

    (

        [UserNo]

    ) REFERENCES [dbo].[Users] (

        [UserID]

    ) ON DELETE CASCADE 

GO



ALTER TABLE [dbo].[UsersTasks] ADD 

    CONSTRAINT [FK_USERSTAS_REF_1064_PROJECT_] FOREIGN KEY 

    (

        [TaskKey]

    ) REFERENCES [dbo].[Project_Tasks] (

        [TaskKey]

    ),

    CONSTRAINT [FK_USERSTAS_REF_1067_USERS] FOREIGN KEY 

    (

        [UserID]

    ) REFERENCES [dbo].[Users] (

        [UserID]

    )

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



/****** Encrypted object is not transferable, and script can not be generated. ******/



GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



/****** Encrypted object is not transferable, and script can not be generated. ******/



GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



/****** Encrypted object is not transferable, and script can not be generated. ******/



GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



/****** Encrypted object is not transferable, and script can not be generated. ******/



GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



/****** Encrypted object is not transferable, and script can not be generated. ******/



GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



/****** Encrypted object is not transferable, and script can not be generated. ******/



GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



/****** Encrypted object is not transferable, and script can not be generated. ******/



GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



/****** Encrypted object is not transferable, and script can not be generated. ******/



GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



/****** Encrypted object is not transferable, and script can not be generated. ******/



GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



/****** Encrypted object is not transferable, and script can not be generated. ******/



GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



/****** Encrypted object is not transferable, and script can not be generated. ******/



GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



/****** Encrypted object is not transferable, and script can not be generated. ******/



GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



/****** Encrypted object is not transferable, and script can not be generated. ******/



GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



/****** Encrypted object is not transferable, and script can not be generated. ******/



GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



/****** Encrypted object is not transferable, and script can not be generated. ******/



GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



CREATE TRIGGER [trg_delete_OpeningStock] ON dbo.OpeningStock 

FOR DELETE

AS

BEGIN

/***** Declaring variables to be inserted *******/

DECLARE @ErrorMsg as varchar(100)

If (@@ROWCOUNT>1)

BEGIN

    set @ErrorMsg= 'Multiple rows cannot be deleted.'

    goto error

END

/****************************************************/

DECLARE @DProductId as varchar(5), @DQty as decimal(11,2), @DBonus as decimal(11,2), @DCost as decimal(18,6),

@CurrentTTLCost as decimal(18,6), @DelTTLCost as decimal(18,6)

/****************************************************/

Select @DProductId=ProductId, @DQty=Round((Qty*Multiplier),2), @DBonus=Round((Bonus*Multiplier),2), @DCost=Cost, @DelTTLCost = (@DQty+@DBonus) * @DCost

From DELETED

/***************************************************/

If not exists (Select ProductId From CurrentStock Where ProductId=@DProductId)

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @DProductId +'.'

    goto error

  END

ELSE If exists (Select ProductId From CurrentStock Where ProductId=@DProductId And Qty<(@DQty+@DBonus))

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @DProductId +'.'

    goto error

  END

ELSE If exists (Select ProductId From CurrentStock Where ProductId=@DProductId And Qty=(@DQty+@DBonus) And abs(Cost-@DCost)>0.1)

  BEGIN

    set @ErrorMsg= 'Access is denied because there is not enough stock to adjust the cost difference for product = '+ @DProductId +'.'

    goto error

  END

ELSE If exists (Select ProductId From CurrentStock Where ProductId=@DProductId And Qty=(@DQty+@DBonus))

  BEGIN

    Delete From CurrentStock Where ProductId=@DProductId

  END

ELSE

  BEGIN

    Select @CurrentTTLCost=Isnull((Qty*Cost),0) From CurrentStock Where ProductId=@DProductId

    /*****************************************************************************************************************/

    Update CurrentStock Set Qty=Qty-(@DQty+@DBonus), 

        Cost=(@CurrentTTLCost - @DelTTLCost) / (Qty-(@DQty+@DBonus))

        Where ProductId=@DProductId

  END

/***************************************************/

return

ERROR:

    raiserror (@ErrorMsg,16,1)

END









GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



CREATE TRIGGER [trg_insert_OpeningStock] ON dbo.OpeningStock 

FOR INSERT

AS

BEGIN

DECLARE @ErrorMsg as varchar(100)

If (@@ROWCOUNT>1)

BEGIN

    set @ErrorMsg= 'Multiple rows cannot be inserted.'

    goto error

END

/***** Declaring variables to be inserted *******/

DECLARE @IProductId as varchar(5), @IQty as decimal(11,2), @IBonus as decimal(11,2), @ICost as decimal(18,6),

@CurrentTTLCost as decimal(18,6), @NewTTLCost as decimal(18,6)

/****************************************************/

Select @IProductId=ProductId, @IQty=Round((Qty*Multiplier),2), @IBonus=Round((Bonus*Multiplier),2), @ICost=Cost, @NewTTLCost = Round((@IQty+@IBonus) * @ICost, 2)

From INSERTED

/***************************************************/

If Not Exists (Select ProductId From CurrentStock Where ProductId=@IProductId)

  BEGIN

    Insert Into CurrentStock(ProductId, Qty, Cost) Values(@IProductId, @IQty+@IBonus, @ICost)

  END

ELSE

  BEGIN

    Select @CurrentTTLCost=Isnull((Qty*Cost),0) From CurrentStock Where ProductId=@IProductId

    /*****************************************************************************************************************/

    Update CurrentStock Set Qty=Qty+(@IQty+@IBonus), 

        Cost=(@CurrentTTLCost + @NewTTLCost) / (Qty+@IQty+@IBonus)

        Where ProductId=@IProductId

  END

/***************************************************/

return

ERROR:

    raiserror (@ErrorMsg,16,1)

END









GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



CREATE TRIGGER [trg_update_OpeningStock] ON dbo.OpeningStock 

FOR UPDATE

AS

BEGIN

DECLARE @ErrorMsg as varchar(100)

If (@@ROWCOUNT>1)

BEGIN

    set @ErrorMsg= 'Multiple rows cannot be updated.'

    goto error

END

/*************CURRENT STOCK QTY*******************************/

DECLARE @CSQty as decimal(11,2), @CSCost as decimal(18,6), @CSTTLCost as decimal(18,6)

/***** Declaring variables to be inserted *******/

DECLARE @IProductId as varchar(5), @IQty as decimal(11,2), @IBonus as decimal(11,2), @ICost as decimal(18,6),

@NewTTLCost as decimal(18,6)

/***** Declaring variables to be Deleted *******/

DECLARE @DProductId as varchar(5), @DQty as decimal(11,2), @DBonus as decimal(11,2), @DCost as decimal(18,6),

@DelTTLCost as decimal(18,6)

/**************************************************************************/

Select @IProductId=ProductId, @IQty=Round((Qty*Multiplier),2), @IBonus=Round((Bonus*Multiplier),2), @ICost=Cost, @NewTTLCost = Round((@IQty+@IBonus) * @ICost, 6)

From INSERTED

/***************************************************/

Select @DProductId=ProductId, @DQty=Round((Qty*Multiplier),2), @DBonus=Round((Bonus*Multiplier),2), @DCost=Cost, @DelTTLCost = (@DQty+@DBonus) * @DCost

From DELETED

/***************************************************/

Select @CSQty=Qty, @CSCost=Cost, @CSTTLCost=Qty*Cost From CurrentStock Where ProductId=@DProductId

/***************************************************/

If not exists (Select ProductId From CurrentStock Where ProductId=@DProductId)

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @DProductId +'.'

    goto error

  END

ELSE If (@CSQty - (@DQty+@DBonus) + (@IQty+@IBonus) < 0)

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @DProductId +'.'

    goto error

  END

ELSE If (@CSQty - (@DQty+@DBonus) + (@IQty+@IBonus) = 0)

  BEGIN

    If (@CSTTLCost - @DelTTLCost + @NewTTLCost<>0) 

      BEGIN

        set @ErrorMsg= 'Updation is denied becuase current stock cost and deleted product cost is not same for product = '+ @DProductId +'.'

        goto error

      END

    ELSE

      BEGIN

        Delete From CurrentStock Where ProductId=@IProductId

      END

  END

ELSE

  BEGIN

    If Not Exists(Select 1 From CurrentStock Where ProductId=@IProductId)

      BEGIN

        Insert Into CurrentStock(ProductId, Qty, Cost) Values(@IProductId, @IQty+@IBonus, @ICost)

      END

    ELSE

      BEGIN

        Update CurrentStock Set Qty=Qty - (@DQty+@DBonus) + (@IQty+@IBonus), 

        Cost=(@CSTTLCost - @DelTTLCost + @NewTTLCost) / (Qty - (@DQty+@DBonus) + (@IQty+@IBonus))

        Where ProductId=@IProductId

      END

  END

/*

If not exists (Select ProductId From CurrentStock Where ProductId=@DProductId)

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @DProductId +'.'

    goto error

  END

ELSE If (@CSQty - (@DQty+@DBonus) + (@IQty+@IBonus) <  (@IQty+@IBonus))

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @DProductId +'.'

    goto error

  END

ELSE

  BEGIN

    Update CurrentStock Set Qty=Qty - (@DQty+@DBonus) + (@IQty+@IBonus), 

        Cost=(@CSTTLCost - @DelTTLCost + @NewTTLCost) / (Qty - (@DQty+@DBonus) + (@IQty+@IBonus))

        Where ProductId=@IProductId

  END

*/

/***************************************************/

return

ERROR:

    raiserror (@ErrorMsg,16,1)

END









GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



/****** Encrypted object is not transferable, and script can not be generated. ******/



GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



/****** Encrypted object is not transferable, and script can not be generated. ******/



GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



CREATE TRIGGER [trg_insert_PurReturnsBody] ON dbo.PurReturnsBody 

FOR INSERT

AS

BEGIN

/***** Declaring variables to be inserted *******/

DECLARE @ErrorMsg as varchar(100)

If (@@ROWCOUNT>1)

BEGIN

    set @ErrorMsg= 'Multiple rows cannot be deleted.'

    goto error

END

/****************************************************/

DECLARE @IProductId as varchar(5), @IQty as decimal(11,2), @IBonus as decimal(11,2), @ICost as decimal(18,6),

@CurrentTTLCost as decimal(18,6), @NewTTLCost as decimal(18,6)

/****************************************************/

Select @IProductId=ProductId, @IQty=Round((Qty*Multiplier),2), @IBonus=Round((Bonus*Multiplier),2), @ICost=Cost, @NewTTLCost = (@IQty+@IBonus) * @ICost

From INSERTED

/***************************************************/

If not exists (Select ProductId From CurrentStock Where ProductId=@IProductId)

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @IProductId +'.'

    goto error

  END

ELSE If exists (Select ProductId From CurrentStock Where ProductId=@IProductId And Qty<(@IQty+@IBonus))

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @IProductId +'.'

    goto error

  END

ELSE If exists (Select ProductId From CurrentStock Where ProductId=@IProductId And Qty=(@IQty+@IBonus) And abs(Cost-@ICost)>0.1)

  BEGIN

    set @ErrorMsg= 'Access is denied because there is not enough stock to adjust the cost difference for product = '+ @IProductId +'.'

    goto error

  END

ELSE If exists (Select ProductId From CurrentStock Where ProductId=@IProductId And Qty=(@IQty+@IBonus))

  BEGIN

    Delete From CurrentStock Where ProductId=@IProductId

  END

ELSE

  BEGIN

    Select @CurrentTTLCost=Isnull((Qty*Cost),0) From CurrentStock Where ProductId=@IProductId

    /*****************************************************************************************************************/

    Update CurrentStock Set Qty=Qty-(@IQty+@IBonus), 

        Cost=(@CurrentTTLCost - @NewTTLCost) / (Qty-(@IQty+@IBonus))

        Where ProductId=@IProductId

  END

/***************************************************/

return

ERROR:

    raiserror (@ErrorMsg,16,1)

END







GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



CREATE TRIGGER [trg_update_PurReturnsBody] ON dbo.PurReturnsBody 

FOR UPDATE

AS

BEGIN

DECLARE @ErrorMsg as varchar(100)

If (@@ROWCOUNT>1)

BEGIN

    set @ErrorMsg= 'Multiple rows cannot be updated.'

    goto error

END

/*************CURRENT STOCK QTY*******************************/

DECLARE @CSQty as int,@CSCost as decimal(18,6),  @CSTTLCost as decimal(18,6)

/***** Declaring variables to be DELETED *******/

DECLARE @DProductId as varchar(5), @DQty as decimal(11,2), @DBonus as decimal(11,2), @DCost as decimal(18,6),

@DelTTLCost as decimal(18,6)

/***** Declaring variables to be INSERTED *******/

DECLARE @IProductId as varchar(5), @IQty as decimal(11,2), @IBonus as decimal(11,2), @ICost as decimal(18,6),

@NewTTLCost as decimal(18,6)

/**************************************************************************/

Select @DProductId=ProductId, @DQty=Round((Qty*Multiplier),2), @DBonus=Round((Bonus*Multiplier),2), @DCost=Cost, @DelTTLCost = Round((@DQty+@DBonus) * @DCost, 6)

From DELETED

/***************************************************/

Select @IProductId=ProductId, @IQty=Round((Qty*Multiplier),2), @IBonus=Round((Bonus*Multiplier),2), @ICost=Cost, @NewTTLCost = (@IQty+@IBonus) * @ICost

From INSERTED

/***************************************************/

Select @CSQty=Qty, @CSCost=Cost, @CSTTLCost=Qty*Cost From CurrentStock Where ProductId=@IProductId

/***************************************************/

If not exists (Select ProductId From CurrentStock Where ProductId=@IProductId)

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @IProductId +'.'

    goto error

  END

ELSE If (@CSQty - (@IQty+@IBonus) + (@DQty+@DBonus) <0)

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @IProductId +'.'

    goto error

  END

ELSE If (@CSQty - (@IQty+@IBonus) + (@DQty+@DBonus) = 0)

  BEGIN

    If (@CSTTLCost - @NewTTLCost + @DelTTLCost<>0) 

      BEGIN

        set @ErrorMsg= 'Updation is denied becuase current stock cost and deleted product cost is not same for product = '+ @IProductId +'.'

        goto error

      END

    ELSE

      BEGIN

        Delete From CurrentStock Where ProductId=@IProductId

      END

  END

ELSE

  BEGIN

    If Not Exists(Select 1 From CurrentStock Where ProductId=@IProductId)

      BEGIN

        Insert Into CurrentStock(ProductId, Qty, Cost) Values(@IProductId, @IQty+@IBonus, @ICost)

      END

    ELSE

      BEGIN

        Update CurrentStock Set Qty=Qty - (@IQty+@IBonus) + (@DQty+@DBonus), 

        Cost=(@CSTTLCost - @NewTTLCost + @DelTTLCost) / (Qty - (@IQty+@IBonus) + (@DQty+@DBonus))

        Where ProductId=@IProductId

      END

  END

/*

If not exists (Select ProductId From CurrentStock Where ProductId=@DProductId)

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @IProductId +'.'

    goto error

  END

ELSE If (@CSQty - (@IQty+@IBonus) + (@DQty+@DBonus) <  (@DQty+@DBonus))

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @IProductId +'.'

    goto error

  END

ELSE

  BEGIN

    Update CurrentStock Set Qty=Qty - (@IQty+@IBonus) + (@DQty+@DBonus), 

        Cost=(@CSTTLCost - @NewTTLCost + @DelTTLCost) / (Qty - (@IQty+@IBonus) + (@DQty+@DBonus))

        Where ProductId=@IProductId

  END

*/

/***************************************************/

return

ERROR:

    raiserror (@ErrorMsg,16,1)

END







GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



CREATE TRIGGER [trg_delete_PurReturnsBody] ON dbo.PurReturnsBody 

FOR DELETE

AS

BEGIN

DECLARE @ErrorMsg as varchar(100)

If (@@ROWCOUNT>1)

BEGIN

    set @ErrorMsg= 'Multiple rows cannot be inserted.'

    goto error

END

/***** Declaring variables to be inserted *******/

DECLARE @DProductId as varchar(5), @DQty as decimal(11,2), @DBonus as decimal(11,2), @DCost as decimal(18,6),

@CurrentTTLCost as decimal(18,6), @DelTTLCost as decimal(18,6)

/****************************************************/

Select @DProductId=ProductId, @DQty=Round((Qty*Multiplier),2), @DBonus=Round((Bonus*Multiplier),2), @DCost=Cost, @DelTTLCost = Round((@DQty+@DBonus) * @DCost, 2)

From DELETED

/***************************************************/

If Not Exists (Select ProductId From CurrentStock Where ProductId=@DProductId)

  BEGIN

    Insert Into CurrentStock(ProductId, Qty, Cost) Values(@DProductId, @DQty+@DBonus, @DCost)

  END

ELSE

  BEGIN

    Select @CurrentTTLCost=Isnull((Qty*Cost),0) From CurrentStock Where ProductId=@DProductId

    /*****************************************************************************************************************/

    Update CurrentStock Set Qty=Qty+(@DQty+@DBonus), 

        Cost=(@CurrentTTLCost + @DelTTLCost) / (Qty+@DQty+@DBonus)

        Where ProductId=@DProductId

  END

/***************************************************/

return

ERROR:

    raiserror (@ErrorMsg,16,1)

END







GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



CREATE TRIGGER [trg_insert_PurchaseBody] ON dbo.PurchaseBody 

FOR INSERT

AS

BEGIN

DECLARE @ErrorMsg as varchar(100)

If (@@ROWCOUNT>1)

BEGIN

    set @ErrorMsg= 'Multiple rows cannot be inserted.'

    goto error

END

/***** Declaring variables to be inserted *******/

DECLARE @IProductId as varchar(5), @IQty as decimal(11,2), @IBonus as decimal(11,2), @ICost as decimal(18,6),

@CurrentTTLCost as decimal(18,6), @NewTTLCost as decimal(18,6)

/****************************************************/

Select @IProductId=ProductId, @IQty=Round((Qty*Multiplier),2), @IBonus=Round((Bonus*Multiplier),2), @ICost=Cost, @NewTTLCost = Round((@IQty+@IBonus) * @ICost, 2)

From INSERTED

/***************************************************/

If Not Exists (Select ProductId From CurrentStock Where ProductId=@IProductId)

  BEGIN

    Insert Into CurrentStock(ProductId, Qty, Cost) Values(@IProductId, @IQty+@IBonus, @ICost)

  END

ELSE

  BEGIN

    Select @CurrentTTLCost=Isnull((Qty*Cost),0) From CurrentStock Where ProductId=@IProductId

    /*****************************************************************************************************************/

    Update CurrentStock Set Qty=Qty+(@IQty+@IBonus), 

        Cost=(@CurrentTTLCost + @NewTTLCost) / (Qty+@IQty+@IBonus)

        Where ProductId=@IProductId

  END

/***************************************************/

return

ERROR:

    raiserror (@ErrorMsg,16,1)

END







GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



CREATE TRIGGER [trg_update_PurchaseBody] ON dbo.PurchaseBody 

FOR UPDATE

AS

BEGIN

DECLARE @ErrorMsg as varchar(100)

If (@@ROWCOUNT>1)

BEGIN

    set @ErrorMsg= 'Multiple rows cannot be updated.'

    goto error

END

/*************CURRENT STOCK QTY*******************************/

DECLARE @CSQty as decimal(11,2), @CSCost as decimal(18,6), @CSTTLCost as decimal(18,6)

/***** Declaring variables to be inserted *******/

DECLARE @IProductId as varchar(5), @IQty as decimal(11,2), @IBonus as decimal(11,2), @ICost as decimal(18,6),

@NewTTLCost as decimal(18,6)

/***** Declaring variables to be Deleted *******/

DECLARE @DProductId as varchar(5), @DQty as decimal(11,2), @DBonus as decimal(11,2), @DCost as decimal(18,6),

@DelTTLCost as decimal(18,6)

/**************************************************************************/

Select @IProductId=ProductId, @IQty=Round((Qty*Multiplier),2), @IBonus=Round((Bonus*Multiplier),2), @ICost=Cost, @NewTTLCost = Round((@IQty+@IBonus) * @ICost, 6)

From INSERTED

/***************************************************/

Select @DProductId=ProductId, @DQty=Round((Qty*Multiplier),2), @DBonus=Round((Bonus*Multiplier),2), @DCost=Cost, @DelTTLCost = (@DQty+@DBonus) * @DCost

From DELETED

/***************************************************/

Select @CSQty=Qty, @CSCost=Cost, @CSTTLCost=Qty*Cost From CurrentStock Where ProductId=@DProductId

/***************************************************/

If not exists (Select ProductId From CurrentStock Where ProductId=@DProductId)

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @DProductId +'.'

    goto error

  END

ELSE If (@CSQty - (@DQty+@DBonus) + (@IQty+@IBonus) < 0)

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @DProductId +'.'

    goto error

  END

ELSE If (@CSQty - (@DQty+@DBonus) + (@IQty+@IBonus) = 0)

  BEGIN

    If (@CSTTLCost - @DelTTLCost + @NewTTLCost<>0) 

      BEGIN

        set @ErrorMsg= 'Updation is denied becuase current stock cost and deleted product cost is not same for product = '+ @DProductId +'.'

        goto error

      END

    ELSE

      BEGIN

        Delete From CurrentStock Where ProductId=@IProductId

      END

  END

ELSE

  BEGIN

    If Not Exists(Select 1 From CurrentStock Where ProductId=@IProductId)

      BEGIN

        Insert Into CurrentStock(ProductId, Qty, Cost) Values(@IProductId, @IQty+@IBonus, @ICost)

      END

    ELSE

      BEGIN

        Update CurrentStock Set Qty=Qty - (@DQty+@DBonus) + (@IQty+@IBonus), 

        Cost=(@CSTTLCost - @DelTTLCost + @NewTTLCost) / (Qty - (@DQty+@DBonus) + (@IQty+@IBonus))

        Where ProductId=@IProductId

      END

  END

/*

If not exists (Select ProductId From CurrentStock Where ProductId=@DProductId)

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @DProductId +'.'

    goto error

  END

ELSE If (@CSQty - (@DQty+@DBonus) + (@IQty+@IBonus) <  (@IQty+@IBonus))

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @DProductId +'.'

    goto error

  END

ELSE

  BEGIN

    Update CurrentStock Set Qty=Qty - (@DQty+@DBonus) + (@IQty+@IBonus), 

        Cost=(@CSTTLCost - @DelTTLCost + @NewTTLCost) / (Qty - (@DQty+@DBonus) + (@IQty+@IBonus))

        Where ProductId=@IProductId

  END

*/

/***************************************************/

return

ERROR:

    raiserror (@ErrorMsg,16,1)

END







GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



CREATE TRIGGER [trg_delete_PurchaseBody] ON dbo.PurchaseBody 

FOR DELETE

AS

BEGIN

/***** Declaring variables to be inserted *******/

DECLARE @ErrorMsg as varchar(100)

If (@@ROWCOUNT>1)

BEGIN

    set @ErrorMsg= 'Multiple rows cannot be deleted.'

    goto error

END

/****************************************************/

DECLARE @DProductId as varchar(5), @DQty as decimal(11,2), @DBonus as decimal(11,2), @DCost as decimal(18,6),

@CurrentTTLCost as decimal(18,6), @DelTTLCost as decimal(18,6)

/****************************************************/

Select @DProductId=ProductId, @DQty=Round((Qty*Multiplier),2), @DBonus=Round((Bonus*Multiplier),2), @DCost=Cost, @DelTTLCost = (@DQty+@DBonus) * @DCost

From DELETED

/***************************************************/

If not exists (Select ProductId From CurrentStock Where ProductId=@DProductId)

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @DProductId +'.'

    goto error

  END

ELSE If exists (Select ProductId From CurrentStock Where ProductId=@DProductId And Qty<(@DQty+@DBonus))

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @DProductId +'.'

    goto error

  END

ELSE If exists (Select ProductId From CurrentStock Where ProductId=@DProductId And Qty=(@DQty+@DBonus) And abs(Cost-@DCost)>0.1)

  BEGIN

    set @ErrorMsg= 'Access is denied because there is not enough stock to adjust the cost difference for product = '+ @DProductId +'.'

    goto error

  END

ELSE If exists (Select ProductId From CurrentStock Where ProductId=@DProductId And Qty=(@DQty+@DBonus))

  BEGIN

    Delete From CurrentStock Where ProductId=@DProductId

  END

ELSE

  BEGIN

    Select @CurrentTTLCost=Isnull((Qty*Cost),0) From CurrentStock Where ProductId=@DProductId

    /*****************************************************************************************************************/

    Update CurrentStock Set Qty=Qty-(@DQty+@DBonus), 

        Cost=(@CurrentTTLCost - @DelTTLCost) / (Qty-(@DQty+@DBonus))

        Where ProductId=@DProductId

  END

/***************************************************/

return

ERROR:

    raiserror (@ErrorMsg,16,1)

END







GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



CREATE TRIGGER [trg_insert_SaleReturnsBody] ON dbo.SaleReturnsBody 

FOR INSERT

AS

BEGIN

DECLARE @ErrorMsg as varchar(100)

If (@@ROWCOUNT>1)

BEGIN

    set @ErrorMsg= 'Multiple rows cannot be inserted.'

    goto error

END

/***** Declaring variables to be inserted *******/

DECLARE @IProductId as varchar(5), @IQty as decimal(11,2), @IBonus as decimal(11,2), @ICost as decimal(18,6),

@CurrentTTLCost as decimal(18,6), @NewTTLCost as decimal(18,6)

/****************************************************/

Select @IProductId=ProductId, @IQty=Round((Qty*Multiplier),2), @IBonus=Round((Bonus*Multiplier),2), @ICost=Cost, @NewTTLCost = Round((@IQty+@IBonus) * @ICost, 2)

From INSERTED

/***************************************************/

If Not Exists (Select ProductId From CurrentStock Where ProductId=@IProductId)

  BEGIN

    Insert Into CurrentStock(ProductId, Qty, Cost) Values(@IProductId, @IQty+@IBonus, @ICost)

  END

ELSE

  BEGIN

    Select @CurrentTTLCost=Isnull((Qty*Cost),0) From CurrentStock Where ProductId=@IProductId

    /*****************************************************************************************************************/

    Update CurrentStock Set Qty=Qty+(@IQty+@IBonus), 

        Cost=(@CurrentTTLCost + @NewTTLCost) / (Qty+@IQty+@IBonus)

        Where ProductId=@IProductId

  END

/***************************************************/

return

ERROR:

    raiserror (@ErrorMsg,16,1)

END









GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



CREATE TRIGGER [trg_update_SaleReturnsBody] ON dbo.SaleReturnsBody 

FOR UPDATE

AS

BEGIN

DECLARE @ErrorMsg as varchar(100)

If (@@ROWCOUNT>1)

BEGIN

    set @ErrorMsg= 'Multiple rows cannot be updated.'

    goto error

END

/*************CURRENT STOCK QTY*******************************/

DECLARE @CSQty as decimal(11,2), @CSCost as decimal(18,6), @CSTTLCost as decimal(18,6)

/***** Declaring variables to be inserted *******/

DECLARE @IProductId as varchar(5), @IQty as decimal(11,2), @IBonus as decimal(11,2), @ICost as decimal(18,6),

@NewTTLCost as decimal(18,6)

/***** Declaring variables to be Deleted *******/

DECLARE @DProductId as varchar(5), @DQty as decimal(11,2), @DBonus as decimal(11,2), @DCost as decimal(18,6),

@DelTTLCost as decimal(18,6)

/**************************************************************************/

Select @IProductId=ProductId, @IQty=Round((Qty*Multiplier),2), @IBonus=Round((Bonus*Multiplier),2), @ICost=Cost, @NewTTLCost = Round((@IQty+@IBonus) * @ICost, 6)

From INSERTED

/***************************************************/

Select @DProductId=ProductId, @DQty=Round((Qty*Multiplier),2), @DBonus=Round((Bonus*Multiplier),2), @DCost=Cost, @DelTTLCost = (@DQty+@DBonus) * @DCost

From DELETED

/***************************************************/

Select @CSQty=Qty, @CSCost=Cost, @CSTTLCost=Qty*Cost From CurrentStock Where ProductId=@DProductId

/***************************************************/

If not exists (Select ProductId From CurrentStock Where ProductId=@DProductId)

  BEGIN

    set @ErrorMsg= 'U1-Sufficient stock not available for product = '+ @DProductId +'.'

    goto error

  END

ELSE If (@CSQty - (@DQty+@DBonus) + (@IQty+@IBonus) < 0)

  BEGIN

    set @ErrorMsg= 'U2-Sufficient stock not available for product = '+ @DProductId +'.'

    goto error

  END

ELSE If (@CSQty - (@DQty+@DBonus) + (@IQty+@IBonus) = 0)

  BEGIN

    If (@CSTTLCost - @DelTTLCost + @NewTTLCost<>0) 

      BEGIN

        set @ErrorMsg= 'U3-Updation is denied becuase current stock cost and deleted product cost is not same for product = '+ @DProductId +'.'

        goto error

      END

    ELSE

      BEGIN

        Delete From CurrentStock Where ProductId=@IProductId

      END

  END

ELSE

  BEGIN

    If Not Exists(Select 1 From CurrentStock Where ProductId=@IProductId)

      BEGIN

        Insert Into CurrentStock(ProductId, Qty, Cost) Values(@IProductId, @IQty+@IBonus, @ICost)

      END

    ELSE

      BEGIN

        Update CurrentStock Set Qty=Qty - (@DQty+@DBonus) + (@IQty+@IBonus), 

        Cost=(@CSTTLCost - @DelTTLCost + @NewTTLCost) / (Qty - (@DQty+@DBonus) + (@IQty+@IBonus))

        Where ProductId=@IProductId

      END

  END

/*

If not exists (Select ProductId From CurrentStock Where ProductId=@DProductId)

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @DProductId +'.'

    goto error

  END

ELSE If (@CSQty - (@DQty+@DBonus) + (@IQty+@IBonus) <  (@IQty+@IBonus))

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @DProductId +'.'

    goto error

  END

ELSE

  BEGIN

    Update CurrentStock Set Qty=Qty - (@DQty+@DBonus) + (@IQty+@IBonus), 

        Cost=(@CSTTLCost - @DelTTLCost + @NewTTLCost) / (Qty - (@DQty+@DBonus) + (@IQty+@IBonus))

        Where ProductId=@IProductId

  END

*/

/***************************************************/

return

ERROR:

    raiserror (@ErrorMsg,16,1)

END











GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



CREATE TRIGGER [trg_delete_SaleReturnsBody] ON dbo.SaleReturnsBody 

FOR DELETE

AS

BEGIN

/***** Declaring variables to be inserted *******/

DECLARE @ErrorMsg as varchar(100)

If (@@ROWCOUNT>1)

BEGIN

    set @ErrorMsg= 'Multiple rows cannot be deleted.'

    goto error

END

/****************************************************/

DECLARE @DProductId as varchar(5), @DQty as decimal(11,2), @DBonus as decimal(11,2), @DCost as decimal(18,6),

@CurrentTTLCost as decimal(18,6), @DelTTLCost as decimal(18,6)

/****************************************************/

Select @DProductId=ProductId, @DQty=Round((Qty*Multiplier),2), @DBonus=Round((Bonus*Multiplier),2), @DCost=Cost, @DelTTLCost = (@DQty+@DBonus) * @DCost

From DELETED

/***************************************************/

If not exists (Select ProductId From CurrentStock Where ProductId=@DProductId)

  BEGIN

    set @ErrorMsg= 'D1-Sufficient stock not available for product = '+ @DProductId +'.'

    goto error

  END

ELSE If exists (Select ProductId From CurrentStock Where ProductId=@DProductId And Qty<(@DQty+@DBonus))

  BEGIN

    set @ErrorMsg= 'D2-Sufficient stock not available for product = '+ @DProductId +'.'

    goto error

  END

ELSE If exists (Select ProductId From CurrentStock Where ProductId=@DProductId And Qty=(@DQty+@DBonus) And abs(Cost-@DCost)>0.1)

  BEGIN

    set @ErrorMsg= 'D3-Access is denied because there is not enough stock to adjust the cost difference for product = '+ @DProductId +'.'

    goto error

  END

ELSE If exists (Select ProductId From CurrentStock Where ProductId=@DProductId And Qty=(@DQty+@DBonus))

  BEGIN

    Delete From CurrentStock Where ProductId=@DProductId

  END

ELSE

  BEGIN

    Select @CurrentTTLCost=Isnull((Qty*Cost),0) From CurrentStock Where ProductId=@DProductId

    /*****************************************************************************************************************/

    Update CurrentStock Set Qty=Qty-(@DQty+@DBonus), 

        Cost=(@CurrentTTLCost - @DelTTLCost) / (Qty-(@DQty+@DBonus))

        Where ProductId=@DProductId

  END

/***************************************************/

return

ERROR:

    raiserror (@ErrorMsg,16,1)

END











GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



CREATE TRIGGER [trg_insert_SalesBody] ON dbo.SalesBody 

FOR INSERT

AS

BEGIN

/***** Declaring variables to be inserted *******/

DECLARE @ErrorMsg as varchar(100)

If (@@ROWCOUNT>1)

BEGIN

    set @ErrorMsg= 'Multiple rows cannot be deleted.'

    goto error

END

/****************************************************/

DECLARE @IProductId as varchar(5), @IQty as decimal(11,2), @IBonus as decimal(11,2), @ICost as decimal(18,6),

@CurrentTTLCost as decimal(18,6), @NewTTLCost as decimal(18,6)

/****************************************************/

Select @IProductId=ProductId, @IQty=Round((Qty*Multiplier),2), @IBonus=Round((Bonus*Multiplier),2), @ICost=Cost, @NewTTLCost = (@IQty+@IBonus) * @ICost

From INSERTED

/***************************************************/

If not exists (Select ProductId From CurrentStock Where ProductId=@IProductId)

  BEGIN

    set @ErrorMsg= 'i1-Sufficient stock not available for product = '+ @IProductId +'.'

    goto error

  END

ELSE If exists (Select ProductId From CurrentStock Where ProductId=@IProductId And Qty<(@IQty+@IBonus))

  BEGIN

    set @ErrorMsg= 'i2-Sufficient stock not available for product = '+ @IProductId +'.' + cast( @IQty+@IBonus as varchar)

    goto error

  END

ELSE If exists (Select ProductId From CurrentStock Where ProductId=@IProductId And Qty=(@IQty+@IBonus) And abs(Cost-@ICost)>0.1)

  BEGIN

    set @ErrorMsg= 'i3-Access is denied because there is not enough stock to adjust the cost difference for product = '+ @IProductId +'.'

    goto error

  END

ELSE If exists (Select ProductId From CurrentStock Where ProductId=@IProductId And Qty=(@IQty+@IBonus))

  BEGIN

    Delete From CurrentStock Where ProductId=@IProductId

  END

ELSE

  BEGIN

    Select @CurrentTTLCost=Isnull((Qty*Cost),0) From CurrentStock Where ProductId=@IProductId

    /*****************************************************************************************************************/

    Update CurrentStock Set Qty=Qty-(@IQty+@IBonus), 

        Cost=(@CurrentTTLCost - @NewTTLCost) / (Qty-(@IQty+@IBonus))

        Where ProductId=@IProductId

  END

/***************************************************/

return

ERROR:

    raiserror (@ErrorMsg,16,1)

END













GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



CREATE TRIGGER [trg_update_SalesBody] ON dbo.SalesBody 

FOR UPDATE

AS

BEGIN

DECLARE @ErrorMsg as varchar(100)

If (@@ROWCOUNT>1)

BEGIN

    set @ErrorMsg= 'Multiple rows cannot be updated.'

    goto error

END

/*************CURRENT STOCK QTY*******************************/

DECLARE @CSQty as int,@CSCost as decimal(18,6),  @CSTTLCost as decimal(18,6)

/***** Declaring variables to be DELETED *******/

DECLARE @DProductId as varchar(5), @DQty as decimal(11,2), @DBonus as decimal(11,2), @DCost as decimal(18,6),

@DelTTLCost as decimal(18,6)

/***** Declaring variables to be INSERTED *******/

DECLARE @IProductId as varchar(5), @IQty as decimal(11,2), @IBonus as decimal(11,2), @ICost as decimal(18,6),

@NewTTLCost as decimal(18,6)

/**************************************************************************/

Select @DProductId=ProductId, @DQty=Round((Qty*Multiplier),2), @DBonus=Round((Bonus*Multiplier),2), @DCost=Cost, @DelTTLCost = Round((@DQty+@DBonus) * @DCost, 6)

From DELETED

/***************************************************/

Select @IProductId=ProductId, @IQty=Round((Qty*Multiplier),2), @IBonus=Round((Bonus*Multiplier),2), @ICost=Cost, @NewTTLCost = (@IQty+@IBonus) * @ICost

From INSERTED

/***************************************************/

Select @CSQty=Qty, @CSCost=Cost, @CSTTLCost=Qty*Cost From CurrentStock Where ProductId=@IProductId

/***************************************************/

If not exists (Select ProductId From CurrentStock Where ProductId=@IProductId)

  BEGIN

    set @ErrorMsg= 'U1-Sufficient stock not available for product = '+ @IProductId +'.'

    goto error

  END

ELSE If (@CSQty - (@IQty+@IBonus) + (@DQty+@DBonus) <0)

  BEGIN

    set @ErrorMsg= 'U2-Sufficient stock not available for product = '+ @IProductId +'.'

    goto error

  END

ELSE If (@CSQty - (@IQty+@IBonus) + (@DQty+@DBonus) = 0)

  BEGIN

    If (@CSTTLCost - @NewTTLCost + @DelTTLCost<>0) 

      BEGIN

        set @ErrorMsg= 'U3-Updation is denied becuase current stock cost and deleted product cost is not same for product = '+ @IProductId +'.'

        goto error

      END

    ELSE

      BEGIN

        Delete From CurrentStock Where ProductId=@IProductId

      END

  END

ELSE

  BEGIN

    If Not Exists(Select 1 From CurrentStock Where ProductId=@IProductId)

      BEGIN

        Insert Into CurrentStock(ProductId, Qty, Cost) Values(@IProductId, @IQty+@IBonus, @ICost)

      END

    ELSE

      BEGIN

        Update CurrentStock Set Qty=Qty - (@IQty+@IBonus) + (@DQty+@DBonus), 

        Cost=(@CSTTLCost - @NewTTLCost + @DelTTLCost) / (Qty - (@IQty+@IBonus) + (@DQty+@DBonus))

        Where ProductId=@IProductId

      END

  END

/*

If not exists (Select ProductId From CurrentStock Where ProductId=@DProductId)

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @IProductId +'.'

    goto error

  END

ELSE If (@CSQty - (@IQty+@IBonus) + (@DQty+@DBonus) <  (@DQty+@DBonus))

  BEGIN

    set @ErrorMsg= 'Sufficient stock not available for product = '+ @IProductId +'.'

    goto error

  END

ELSE

  BEGIN

    Update CurrentStock Set Qty=Qty - (@IQty+@IBonus) + (@DQty+@DBonus), 

        Cost=(@CSTTLCost - @NewTTLCost + @DelTTLCost) / (Qty - (@IQty+@IBonus) + (@DQty+@DBonus))

        Where ProductId=@IProductId

  END

*/

/***************************************************/

return

ERROR:

    raiserror (@ErrorMsg,16,1)

END













GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO



SET QUOTED_IDENTIFIER ON 

GO

SET ANSI_NULLS ON 

GO



CREATE TRIGGER [trg_delete_SalesBody] ON dbo.SalesBody 

FOR DELETE

AS

BEGIN

DECLARE @ErrorMsg as varchar(100)

If (@@ROWCOUNT>1)

BEGIN

    set @ErrorMsg= 'Multiple rows cannot be inserted.'

    goto error

END

/***** Declaring variables to be inserted *******/

DECLARE @DProductId as varchar(5), @DQty as decimal(11,2), @DBonus as decimal(11,2), @DCost as decimal(18,6),

@CurrentTTLCost as decimal(18,6), @DelTTLCost as decimal(18,6)

/****************************************************/

Select @DProductId=ProductId, @DQty=Round((Qty*Multiplier),2), @DBonus=Round((Bonus*Multiplier),2), @DCost=Cost, @DelTTLCost = Round((@DQty+@DBonus) * @DCost, 2)

From DELETED

/***************************************************/

If Not Exists (Select ProductId From CurrentStock Where ProductId=@DProductId)

  BEGIN

    Insert Into CurrentStock(ProductId, Qty, Cost) Values(@DProductId, @DQty+@DBonus, @DCost)

  END

ELSE

  BEGIN

    Select @CurrentTTLCost=Isnull((Qty*Cost),0) From CurrentStock Where ProductId=@DProductId

    /*****************************************************************************************************************/

    Update CurrentStock Set Qty=Qty+(@DQty+@DBonus), 

        Cost=(@CurrentTTLCost + @DelTTLCost) / (Qty+@DQty+@DBonus)

        Where ProductId=@DProductId

  END

/***************************************************/

return

ERROR:

    raiserror (@ErrorMsg,16,1)

END









GO

SET QUOTED_IDENTIFIER OFF 

GO

SET ANSI_NULLS ON 

GO
namespace ZaRan.Services.Languages;

public interface ILanguage
{
    public string UserRegister_UserAlreadyExists { get; }
    public string UserRelationShipAdd_AlreadyExists { get; }
    public string TransactionPatch_NoEnoughMoney { get; }
    public string TransactionPatch_NoEnoughGood { get; }
    public string TransactionPatch_AlreadyDealed { get; }
    public string TransactionPatch_AlreadyCanceled { get; }
}
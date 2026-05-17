namespace ZaRan.Services.Languages;

public class ZhCnLanguage : ILanguage
{
    public string UserRegister_UserAlreadyExists => "用户已经存在";
    public string UserRelationShipAdd_AlreadyExists => "已经关注";
    public string TransactionPatch_NoEnoughMoney => "余额不足";
    public string TransactionPatch_NoEnoughGood => "已售罄";
    public string TransactionPatch_AlreadyDealed => "交易完成, 无法撤销";
    public string TransactionPatch_AlreadyCanceled => "交易取消, 无法更改";
}
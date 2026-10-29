namespace smartFactoryApp.Models;

// 释放积木的操作结果
public enum UndockResult
{
    Success,           // 释放成功
    NoBrick,           // 没有携带积木
    NotOnDeliveryLine  // 不在交付线
}
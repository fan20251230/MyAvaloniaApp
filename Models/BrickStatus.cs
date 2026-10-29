namespace smartFactoryApp.Models;

public enum BrickStatus
{
    Ready,        // 等待抓取
    ToCustomer,   // 正在送往客户
    ToRecycle,    // 正在送往回收
    AtCustomer,   // 已到客户箭头位置
    AtRecycle,    // 已到回收箭头位置

    ReturningToRecycle   // 从左侧箭头运往中央回收线
}
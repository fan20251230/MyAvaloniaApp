using System.Collections.Generic;
using static smartFactoryApp.Models.Parameters;

namespace smartFactoryApp.Models;

public class Logistics
{
    // 客户需要的数量
    private readonly Dictionary<LegoColor, int> order = new();

    // 已成功交给客户的数量
    private readonly Dictionary<LegoColor, int> delivered = new();

    public bool HasOrder { get; private set; }

    // 整个订单需要的积木总数
    public int TotalOrdered { get; private set; }

    // 已成功交付的积木总数
    public int TotalDelivered { get; private set; }

    public bool IsOrderComplete =>
        HasOrder && TotalDelivered == TotalOrdered;

    public Logistics()
    {
        ResetCounts();
    }

    private void ResetCounts()
    {
        foreach (LegoColor color in new[]
        {
            LegoColor.yellow,
            LegoColor.blue,
            LegoColor.red,
            LegoColor.green
        })
        {
            order[color] = 0;
            delivered[color] = 0;
        }

        TotalOrdered = 0;
        TotalDelivered = 0;
        HasOrder = false;
    }

    // 创建订单：参数顺序与界面一致
    public void CreateOrder(
        int yellow,
        int blue,
        int red,
        int green)
    {
        ResetCounts();

        order[LegoColor.yellow] = yellow;
        order[LegoColor.blue] = blue;
        order[LegoColor.red] = red;
        order[LegoColor.green] = green;

        TotalOrdered = yellow + blue + red + green;
        HasOrder = TotalOrdered > 0;
    }

    public int GetOrdered(LegoColor color)
    {
        return order[color];
    }

    public int GetDelivered(LegoColor color)
    {
        return delivered[color];
    }

    // 只有成功交给客户时，才调用这个方法
    public bool BrickDelivered(LegoColor color)
    {
        if (!HasOrder || delivered[color] >= order[color])
            return false;

        delivered[color]++;
        TotalDelivered++;

        return true;
    }


    // 当前计划要抓取的积木
public LegoBrick? CurrentBrick { get; private set; }

// 取货来源，例如 "red production line" 或 "recycle line"
public string PickStation { get; private set; } = "";

// 根据剩余订单选择下一块积木
public void PlanNextPickUp(
    IEnumerable<ProductionLine> productionLines,
    IEnumerable<LegoBrick> recycleBricks)
{
    CurrentBrick = null;
    PickStation = "";

    if (!HasOrder || IsOrderComplete)
        return;

    // 优先检查已经回到中央回收线、可以重新抓取的积木
    foreach (LegoBrick brick in recycleBricks)
    {
        if (brick.IsDocked ||
            brick.Status != BrickStatus.Ready ||
            brick.RecyclePlatz < 0)
        {
            continue;
        }

        if (GetDelivered(brick.Color) < GetOrdered(brick.Color))
        {
            CurrentBrick = brick;
            PickStation = "recycle line";
            return;
        }
    }

    // 再检查四条生产线
    foreach (ProductionLine line in productionLines)
    {
        foreach (LegoBrick brick in line.Bricks)
        {
            if (brick.IsDocked)
                continue;

            if (GetDelivered(brick.Color) < GetOrdered(brick.Color))
            {
                CurrentBrick = brick;
                PickStation = $"{line.Color} production line";
                return;
            }
        }
    }
}

}
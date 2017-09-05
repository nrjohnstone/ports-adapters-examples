using System;

namespace Core.Entities
{
    public enum BookOrderState
    {
       New,
       AwaitingApproval,
       Approved,
       Sent
    }
}
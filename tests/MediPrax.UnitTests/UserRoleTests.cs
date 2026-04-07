using MediPrax.Core.Enums;

namespace MediPrax.UnitTests;

public class UserRoleTests
{
    [Fact]
    public void UserRole_HasExpectedValues()
    {
        Assert.Equal(0, (int)UserRole.Arzt);
        Assert.Equal(1, (int)UserRole.MFA);
        Assert.Equal(2, (int)UserRole.Empfang);
        Assert.Equal(3, (int)UserRole.Admin);
    }

    [Fact]
    public void BillingType_HasExpectedValues()
    {
        Assert.Equal(0, (int)BillingType.EBM);
        Assert.Equal(1, (int)BillingType.GOA);
    }

    [Fact]
    public void AuditAction_HasAllExpectedValues()
    {
        var values = Enum.GetValues<AuditAction>();
        Assert.Contains(AuditAction.Create, values);
        Assert.Contains(AuditAction.Read, values);
        Assert.Contains(AuditAction.Update, values);
        Assert.Contains(AuditAction.Delete, values);
        Assert.Contains(AuditAction.Login, values);
        Assert.Contains(AuditAction.LoginFailed, values);
        Assert.Contains(AuditAction.Logout, values);
    }
}

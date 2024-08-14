﻿namespace MultiTenantDbApp.Domain.Master.Entities;
public sealed class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string ConnectionString { get; set; } = default!;
}

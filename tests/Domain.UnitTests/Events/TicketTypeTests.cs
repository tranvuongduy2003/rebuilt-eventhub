using EventHub.Domain.Events;
using EventHub.Domain.Exceptions;
using EventHub.Domain.Users;
using FluentAssertions;

namespace EventHub.Domain.UnitTests.Events;

public sealed class TicketTypeTests
{
    private static readonly DateTimeOffset CreatedAt = new(2026, 7, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset StartsAt = new(2026, 7, 15, 14, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset EndsAt = new(2026, 7, 15, 16, 0, 0, TimeSpan.Zero);

    // --- TicketName.Create ---

    [Fact]
    public void TicketName_ValidInput_CreatesTicketName()
    {
        var name = TicketName.Create("General Admission");

        name.Value.Should().Be("General Admission");
    }

    [Fact]
    public void TicketName_TrimsWhitespace()
    {
        var name = TicketName.Create("  VIP  ");

        name.Value.Should().Be("VIP");
    }

    [Fact]
    public void TicketName_EmptyString_ThrowsBusinessRuleValidationException()
    {
        var act = () => TicketName.Create("");

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Code.Should().Be("INVALID_TICKET_TYPE_NAME");
    }

    [Fact]
    public void TicketName_WhitespaceOnly_ThrowsBusinessRuleValidationException()
    {
        var act = () => TicketName.Create("   ");

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Code.Should().Be("INVALID_TICKET_TYPE_NAME");
    }

    [Fact]
    public void TicketName_Exceeds200Characters_ThrowsBusinessRuleValidationException()
    {
        var longName = new string('A', 201);

        var act = () => TicketName.Create(longName);

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Code.Should().Be("INVALID_TICKET_TYPE_NAME");
    }

    [Fact]
    public void TicketName_Exactly200Characters_Succeeds()
    {
        var name = new string('A', 200);

        var ticketName = TicketName.Create(name);

        ticketName.Value.Should().HaveLength(200);
    }

    // --- Money.Create ---

    [Fact]
    public void Money_ValidInput_CreatesMoney()
    {
        var money = Money.Create(50.00m, "VND");

        money.Amount.Should().Be(50.00m);
        money.Currency.Should().Be("VND");
    }

    [Fact]
    public void Money_ZeroAmount_CreatesFreeTicket()
    {
        var money = Money.Create(0, "VND");

        money.Amount.Should().Be(0);
    }

    [Fact]
    public void Money_NegativeAmount_ThrowsBusinessRuleValidationException()
    {
        var act = () => Money.Create(-1, "VND");

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Code.Should().Be("INVALID_TICKET_TYPE_PRICE");
    }

    [Fact]
    public void Money_EmptyCurrency_ThrowsBusinessRuleValidationException()
    {
        var act = () => Money.Create(10, "");

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Code.Should().Be("INVALID_TICKET_TYPE_PRICE");
    }

    [Fact]
    public void Money_CurrencyUpperCase()
    {
        var money = Money.Create(10, "vnd");

        money.Currency.Should().Be("VND");
    }

    // --- Capacity.Create ---

    [Fact]
    public void Capacity_ValidInput_CreatesCapacity()
    {
        var capacity = Capacity.Create(100);

        capacity.Value.Should().Be(100);
    }

    [Fact]
    public void Capacity_ZeroValue_ThrowsBusinessRuleValidationException()
    {
        var act = () => Capacity.Create(0);

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Code.Should().Be("INVALID_TICKET_TYPE_CAPACITY");
    }

    [Fact]
    public void Capacity_NegativeValue_ThrowsBusinessRuleValidationException()
    {
        var act = () => Capacity.Create(-1);

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Code.Should().Be("INVALID_TICKET_TYPE_CAPACITY");
    }

    // --- TicketTypeId.From ---

    [Fact]
    public void TicketTypeId_ValidInput_CreatesId()
    {
        var id = TicketTypeId.From(1);

        id.Value.Should().Be(1);
    }

    [Fact]
    public void TicketTypeId_ZeroValue_ThrowsBusinessRuleValidationException()
    {
        var act = () => TicketTypeId.From(0);

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Code.Should().Be("TICKET_TYPE_ID_INVALID");
    }

    [Fact]
    public void TicketTypeId_NegativeValue_ThrowsBusinessRuleValidationException()
    {
        var act = () => TicketTypeId.From(-1);

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Code.Should().Be("TICKET_TYPE_ID_INVALID");
    }

    // --- TicketType.Create ---

    [Fact]
    public void TicketType_Create_SetsProperties()
    {
        var name = TicketName.Create("General Admission");
        var price = Money.Create(50.00m, "VND");
        var capacity = Capacity.Create(100);

        var ticketType = TicketType.Create(name, price, capacity, CreatedAt);

        ticketType.Name.Should().Be(name);
        ticketType.Price.Should().Be(price);
        ticketType.Capacity.Should().Be(capacity);
        ticketType.Sold.Should().Be(0);
        ticketType.Reserved.Should().Be(0);
        ticketType.CreatedAt.Should().Be(CreatedAt);
        ticketType.UpdatedAt.Should().Be(CreatedAt);
    }

    // --- TicketType.FromPersistence ---

    [Fact]
    public void TicketType_FromPersistence_SetsAllProperties()
    {
        var ticketType = TicketType.FromPersistence(
            TicketTypeId.From(42),
            TicketName.Create("VIP"),
            Money.Create(100m, "VND"),
            Capacity.Create(50),
            10,
            5,
            CreatedAt,
            CreatedAt);

        ticketType.Id.Value.Should().Be(42);
        ticketType.Name.Value.Should().Be("VIP");
        ticketType.Price.Amount.Should().Be(100m);
        ticketType.Capacity.Value.Should().Be(50);
        ticketType.Sold.Should().Be(10);
        ticketType.Reserved.Should().Be(5);
    }

    // --- Event.AddTicketType ---

    [Fact]
    public void AddTicketType_ValidInput_AddsToCollection()
    {
        var draftEvent = CreateValidDraftEvent();
        var name = TicketName.Create("General Admission");
        var price = Money.Create(50.00m, "VND");
        var capacity = Capacity.Create(100);

        var ticketType = draftEvent.AddTicketType(name, price, capacity, CreatedAt);

        draftEvent.TicketTypes.Should().ContainSingle()
            .Which.Should().Be(ticketType);
    }

    [Fact]
    public void AddTicketType_ValidInput_RaisesTicketTypeAddedEvent()
    {
        var draftEvent = CreateValidDraftEvent();
        var name = TicketName.Create("General Admission");
        var price = Money.Create(50.00m, "VND");
        var capacity = Capacity.Create(100);

        draftEvent.AddTicketType(name, price, capacity, CreatedAt);

        draftEvent.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<TicketTypeAddedEvent>();
    }

    [Fact]
    public void AddTicketType_ValidInput_UpdatesEventTimestamp()
    {
        var draftEvent = CreateValidDraftEvent();
        var name = TicketName.Create("General Admission");
        var price = Money.Create(50.00m, "VND");
        var capacity = Capacity.Create(100);

        draftEvent.AddTicketType(name, price, capacity, CreatedAt);

        draftEvent.UpdatedAt.Should().Be(CreatedAt);
    }

    [Fact]
    public void AddTicketType_FreeTicket_Succeeds()
    {
        var draftEvent = CreateValidDraftEvent();
        var name = TicketName.Create("Free Entry");
        var price = Money.Create(0, "VND");
        var capacity = Capacity.Create(100);

        var ticketType = draftEvent.AddTicketType(name, price, capacity, CreatedAt);

        ticketType.Price.Amount.Should().Be(0);
        draftEvent.TicketTypes.Should().ContainSingle();
    }

    [Fact]
    public void AddTicketType_PublishedEvent_ThrowsBusinessRuleValidationException()
    {
        var publishedEvent = CreatePublishedEvent();
        var name = TicketName.Create("VIP");
        var price = Money.Create(100m, "VND");
        var capacity = Capacity.Create(50);

        var act = () => publishedEvent.AddTicketType(name, price, capacity, CreatedAt);

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Code.Should().Be("INVALID_EVENT_STATUS");
    }

    [Fact]
    public void AddTicketType_ClosedEvent_ThrowsBusinessRuleValidationException()
    {
        var closedEvent = CreateClosedEvent();
        var name = TicketName.Create("VIP");
        var price = Money.Create(100m, "VND");
        var capacity = Capacity.Create(50);

        var act = () => closedEvent.AddTicketType(name, price, capacity, CreatedAt);

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Code.Should().Be("INVALID_EVENT_STATUS");
    }

    [Fact]
    public void AddTicketType_CancelledEvent_ThrowsBusinessRuleValidationException()
    {
        var cancelledEvent = CreateCancelledEvent();
        var name = TicketName.Create("VIP");
        var price = Money.Create(100m, "VND");
        var capacity = Capacity.Create(50);

        var act = () => cancelledEvent.AddTicketType(name, price, capacity, CreatedAt);

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Code.Should().Be("INVALID_EVENT_STATUS");
    }

    // --- Event.Publish INV-11 guard ---

    [Fact]
    public void Publish_NoTicketTypes_ThrowsBusinessRuleValidationException()
    {
        var draftEvent = CreateValidDraftEvent();
        var slug = Slug.Create("tech-conference-2026");

        var act = () => draftEvent.Publish(slug, CreatedAt);

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Code.Should().Be("EVENT_REQUIRES_TICKET_TYPE");
    }

    [Fact]
    public void Publish_WithTicketType_Succeeds()
    {
        var draftEvent = CreateValidDraftEvent();
        draftEvent.AddTicketType(
            TicketName.Create("General Admission"),
            Money.Create(50m, "VND"),
            Capacity.Create(100),
            CreatedAt);

        var slug = Slug.Create("tech-conference-2026");
        draftEvent.Publish(slug, CreatedAt);

        draftEvent.Status.Should().Be(EventStatus.Published);
    }

    [Fact]
    public void Publish_WithMultipleTicketTypes_Succeeds()
    {
        var draftEvent = CreateValidDraftEvent();
        draftEvent.AddTicketType(
            TicketName.Create("General Admission"),
            Money.Create(50m, "VND"),
            Capacity.Create(100),
            CreatedAt);
        draftEvent.AddTicketType(
            TicketName.Create("VIP"),
            Money.Create(200m, "VND"),
            Capacity.Create(20),
            CreatedAt);

        var slug = Slug.Create("tech-conference-2026");
        draftEvent.Publish(slug, CreatedAt);

        draftEvent.Status.Should().Be(EventStatus.Published);
    }

    // --- Event.LoadTicketTypes ---

    [Fact]
    public void LoadTicketTypes_SetsTicketTypesOnEvent()
    {
        var draftEvent = CreateValidDraftEvent();
        var ticketTypes = new List<TicketType>
        {
            TicketType.FromPersistence(
                TicketTypeId.From(1),
                TicketName.Create("General"),
                Money.Create(50m, "VND"),
                Capacity.Create(100),
                0, 0, CreatedAt, CreatedAt),
            TicketType.FromPersistence(
                TicketTypeId.From(2),
                TicketName.Create("VIP"),
                Money.Create(200m, "VND"),
                Capacity.Create(20),
                0, 0, CreatedAt, CreatedAt),
        };

        draftEvent.LoadTicketTypes(ticketTypes);

        draftEvent.TicketTypes.Should().HaveCount(2);
    }

    [Fact]
    public void LoadTicketTypes_ReplacesExistingTicketTypes()
    {
        var draftEvent = CreateValidDraftEvent();
        draftEvent.AddTicketType(
            TicketName.Create("Old"),
            Money.Create(10m, "VND"),
            Capacity.Create(10),
            CreatedAt);

        var newTicketTypes = new List<TicketType>
        {
            TicketType.FromPersistence(
                TicketTypeId.From(99),
                TicketName.Create("New"),
                Money.Create(50m, "VND"),
                Capacity.Create(100),
                0, 0, CreatedAt, CreatedAt),
        };

        draftEvent.LoadTicketTypes(newTicketTypes);

        draftEvent.TicketTypes.Should().HaveCount(1);
        draftEvent.TicketTypes.First().Id.Value.Should().Be(99);
    }

    // --- Value object equality ---

    [Fact]
    public void Money_SameValues_AreEqual()
    {
        var money1 = Money.Create(50m, "VND");
        var money2 = Money.Create(50m, "VND");

        money1.Should().Be(money2);
    }

    [Fact]
    public void Money_DifferentAmounts_AreNotEqual()
    {
        var money1 = Money.Create(50m, "VND");
        var money2 = Money.Create(100m, "VND");

        money1.Should().NotBe(money2);
    }

    [Fact]
    public void Capacity_SameValues_AreEqual()
    {
        var cap1 = Capacity.Create(100);
        var cap2 = Capacity.Create(100);

        cap1.Should().Be(cap2);
    }

    [Fact]
    public void TicketName_SameValues_AreEqual()
    {
        var name1 = TicketName.Create("VIP");
        var name2 = TicketName.Create("VIP");

        name1.Should().Be(name2);
    }

    // --- Helper methods ---

    private static Event CreateValidDraftEvent() =>
        Event.CreateDraft(
            UserId.New(),
            EventTitle.Create("Tech Conference 2026"),
            EventSchedule.Create(StartsAt, EndsAt, "UTC"),
            EventLocation.Create("123 Conference Ave", false),
            CreatedAt);

    private static Event CreatePublishedEvent()
    {
        var draftEvent = CreateValidDraftEvent();
        draftEvent.AddTicketType(
            TicketName.Create("General"),
            Money.Create(50m, "VND"),
            Capacity.Create(100),
            CreatedAt);
        draftEvent.Publish(Slug.Create("tech-conference-2026"), CreatedAt);
        return draftEvent;
    }

    private static Event CreateClosedEvent()
    {
        var e = Event.FromPersistence(
            EventId.From(1),
            UserId.New(),
            EventTitle.Create("Closed Event"),
            EventSchedule.Create(StartsAt, EndsAt, "UTC"),
            EventLocation.Create("123 Main St", false),
            null,
            EventStatus.Closed,
            null,
            null,
            null,
            CreatedAt,
            CreatedAt,
            1);
        return e;
    }

    private static Event CreateCancelledEvent()
    {
        var e = Event.FromPersistence(
            EventId.From(1),
            UserId.New(),
            EventTitle.Create("Cancelled Event"),
            EventSchedule.Create(StartsAt, EndsAt, "UTC"),
            EventLocation.Create("123 Main St", false),
            null,
            EventStatus.Cancelled,
            null,
            null,
            CreatedAt,
            CreatedAt,
            CreatedAt,
            1);
        return e;
    }
}

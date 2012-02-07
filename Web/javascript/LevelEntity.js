
function LevelEntity(id, uuid, name, shortDescription, description, colorValue, escalationTimespan, dueTimespan, directContactRequired) {
    this.Id = id;
    this.Name = name;
    this.Description = description;
    this.ShortDescription = shortDescription;
    this.ColorValue = colorValue;
    this.ColorValueHex = colorValue.toString(16).toUpperCase();
    this.EscalationTimeout = escalationTimespan;
    this.DueTimeout = dueTimespan;
    this.Uuid = uuid;
    this.DirectContactRequired = directContactRequired;
}

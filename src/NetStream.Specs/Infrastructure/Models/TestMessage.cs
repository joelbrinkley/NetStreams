using System;

namespace NetStreams.Specs.Infrastructure.Models
{
    public class TestMessage
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string Description { get; set; }

        public string Key => Id;
    }
}

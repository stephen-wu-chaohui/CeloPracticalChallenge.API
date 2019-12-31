using System;

namespace CeloPracticalChallenge.API.ExternalDtos
{
    public class RandomUserInput
    {
        public string email { get; set; }

        public NameInput name { get; set; }

        public DOBInput dob { get; set; }

        public string phone { get; set; }

        public PictureInput picture { get; set; }
    }

    public class NameInput
    {
        public string title { get; set; }
        public string first { get; set; }
        public string last { get; set; }
    }

    public class DOBInput
    {
        public DateTime date { get; set; }
        public int age { get; set; }
    }

    public class PictureInput
    {
        public string large { get; set; }
        public string medium { get; set; }
        public string thumbnail { get; set; }
    }
}

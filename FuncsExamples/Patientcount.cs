namespace FuncsExamples
{
    public class Patientcount
    {
        public int? CareCompanionMessageCount { get; set; }
        public int? LabResultsCount { get; set; }
        public int? LetterCount { get; set; }
        public int? ConversationCount { get; set; }
        public int? AppointmentMessageCount { get; set; }
        public int? TotalUnreadMessages { get; set; }
        public string PatientID { get; set; }
        public string PatientMRN { get; set; }
        public string PatientName { get; set; }
        public bool Requestor { get; set; }
    }
}
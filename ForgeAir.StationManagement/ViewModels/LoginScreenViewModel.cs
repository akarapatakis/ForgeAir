namespace ForgeAir.StationManagement.ViewModels
{ 
    public partial class LoginScreenViewModel : ViewModelBase
    {
#pragma warning disable CA1822 // Mark members as static
        public string Greeting => "Authentication";
        public string CredentialPrompt => "Please Enter your Credentials";
#pragma warning restore CA1822 // Mark members as static
    }
}

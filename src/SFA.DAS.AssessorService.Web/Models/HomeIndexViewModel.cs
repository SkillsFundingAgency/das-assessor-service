namespace SFA.DAS.AssessorService.Web.Models
{
    public class HomeIndexViewModel
    {
        public bool UseGovSignIn { get; set; }
        public string BannerViewPath => Constants.Banners.APARNotifyBannerViewPath;
    }
}

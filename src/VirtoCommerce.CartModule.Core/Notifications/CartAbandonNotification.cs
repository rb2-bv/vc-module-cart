using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.CartModule.Core.Notifications
{
    public class CartAbandonNotification : EmailNotification
    {
        public string CartReferenceURI { get; set; }
        public string CouponCode { get; set; }
    }
}

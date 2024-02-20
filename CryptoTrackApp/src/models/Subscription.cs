using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CryptoTrackApp.src.models
{
    public class Subscription
    {
        private Guid _subscriptionId;
	private Guid _userId;
        private Guid _currencyId;
        private User _user = null!;
	private double _notificationUmbral = 0.5;
	private DateTime _followDate;
        private DateTime? _unfollowDate;

        //public Subscription (Guid pId, Guid pUserId, 
	    //Guid pCurrencyId) {

            //this.SubscriptionId = pId;
            //this.UserId = pUserId;
            //this.CurrencyId = pCurrencyId;
            //this.FollowDate = DateTime.Now;

	    //this.NotificationUmbral = 0.5;
        //}

	public Guid SubscriptionId { get { return this._subscriptionId; } set { this._subscriptionId=value; }}
	public Guid UserId { get {return this._userId; } set { this._userId=value; } }
	public Guid CurrencyId { get { return this._currencyId; } set { this._currencyId=value; } }
	public User User { get { return this._user; } set { this._user=value; } }
	public double NotificationUmbral { get {return this._notificationUmbral; } set { this._notificationUmbral=value; } }
	public DateTime FollowDate { get { return this._followDate; } set { this._followDate=value; }}
	public DateTime? UnfollowDate { get { return this._unfollowDate; } set { this._unfollowDate=value; } }

  }
}

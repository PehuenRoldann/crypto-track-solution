using System;
using System.Collections;
using System.Collections.Generic;
using GLib;

namespace CryptoTrackApp.src.models
{

  public class User
  {
    private Guid _id;
    private String _email;
    private String _password;
    private System.DateTime _birthDate;
    private ICollection<Subscription> _subscriptions;
  

    public Guid Id { get {return this._id;} set {this._id = value;}}
    public String Email { get {return this._email;} set {this._email = value;} }
    public String Password { get {return this._password;} set {this._password = value;} }
    public ICollection<Subscription> Subscriptions { get {return this._subscriptions;} set {this._subscriptions = value;}}

    public System.DateTime BirthDate {get {return this._birthDate;} set {this._birthDate = value;}}

  }
}

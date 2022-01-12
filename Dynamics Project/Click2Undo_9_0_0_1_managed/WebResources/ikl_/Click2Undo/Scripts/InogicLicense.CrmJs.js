'use strict';
var IKL = window.IKL || {};
!function(canCreateDiscussions) {
  /**
   * @return {undefined}
   */
  function self() {
    var $scope;
    ($scope = this).getXrm();
    $scope.getContext();
    $scope.getApiUrl();
  }
  var resultEntry_text_title;
  var title;
  canCreateDiscussions.InogicLicense;
  resultEntry_text_title = canCreateDiscussions.InogicLicense || (canCreateDiscussions.InogicLicense = {});
  /**
   * @return {undefined}
   */
  self.prototype.getXrm = function() {
    if (null != typeof Xrm && "undefined" != typeof Xrm) {
      this._Xrm = Xrm;
    } else {
      if (null == typeof parent.Xrm || void 0 === parent.Xrm) {
        throw new Error("Xrm is not available.");
      }
      this._Xrm = parent.Xrm;
    }
  };
  /**
   * @return {?}
   */
  self.prototype.getContext = function() {
    if (void 0 === this._Xrm) {
      throw new Error("Context is not available.");
    }
    return void 0 !== this._Xrm.Utility && void 0 !== this._Xrm.Utility.getGlobalContext ? this._Context = this._Xrm.Utility.getGlobalContext() : void 0 !== window.GetGlobalContext ? this._Context = window.GetGlobalContext() : void 0 !== this._Xrm.Page && (this._Context = Xrm.Page.context), this._Context;
  };
  /**
   * @return {?}
   */
  self.prototype.getClientUrl = function() {
    var processingFormat = "undefined" != this._Context && null != this._Context && void 0 !== this._Context.getClientUrl ? this._Context.getClientUrl() : this._Context.getServerUrl();
    return processingFormat.match(/\/$/) && (processingFormat = processingFormat.substring(0, processingFormat.length - 1)), processingFormat;
  };
  /**
   * @return {?}
   */
  self.prototype.getApiUrl = function() {
    return this._ApiUrl = this.getClientUrl() + "/api/data/v" + this.getApiVersion() + "/", this._ApiUrl;
  };
  /**
   * @return {?}
   */
  self.prototype.getOrgUniqueName = function() {
    return void 0 !== this._Context && void 0 !== this._Context.organizationSettings && void 0 !== this._Context.organizationSettings.uniqueName ? this._Context.organizationSettings.uniqueName : void 0 !== this._Context.getOrgUniqueName ? this._Context.getOrgUniqueName() : null;
  };
  /**
   * @return {?}
   */
  self.prototype.getVersion = function() {
    return void 0 !== this._Context.getVersion && "undefined" != this._Context.getVersion ? this._Context.getVersion() : "8.0";
  };
  /**
   * @return {?}
   */
  self.prototype.getQueryStringParameters = function() {
    return void 0 !== this._Context && void 0 !== this._Context.getQueryStringParameters ? this._Context.getQueryStringParameters() : null;
  };
  /**
   * @return {?}
   */
  self.prototype.getUserName = function() {
    return void 0 !== this._Context && void 0 !== this._Context.userSettings && void 0 !== this._Context.userSettings.userName ? this._Context.userSettings.userName : void 0 !== this._Context.getUserName ? this._Context.getUserName() : null;
  };
  /**
   * @return {?}
   */
  self.prototype.getUserId = function() {
    return void 0 !== this._Context && void 0 !== this._Context.userSettings && void 0 !== this._Context.userSettings.userId ? this._Context.userSettings.userId : void 0 !== this._Context.getUserId ? this._Context.getUserId() : null;
  };
  /**
   * @return {?}
   */
  self.prototype.getUserLcId = function() {
    return void 0 !== this._Context && void 0 !== this._Context.userSettings && void 0 !== this._Context.userSettings.languageId ? this._Context.userSettings.languageId : void 0 !== this._Context.getUserLcid ? this._Context.getUserLcid() : null;
  };
  /**
   * @return {?}
   */
  self.prototype.getSecurityRoles = function() {
    return void 0 !== this._Context && void 0 !== this._Context.userSettings && void 0 !== this._Context.userSettings.securityRoles ? this._Context.userSettings.securityRoles : void 0 !== this._Context.getUserRoles ? this._Context.getUserRoles() : null;
  };
  /**
   * @return {?}
   */
  self.prototype.getClient = function() {
    if (void 0 !== this._Context && void 0 !== this._Context.client && void 0 !== this._Context.client.getClient) {
      return this._Context.client.getClient();
    }
  };
  /**
   * @return {?}
   */
  self.prototype.getClientState = function() {
    if (void 0 !== this._Context && void 0 !== this._Context.client && void 0 !== this._Context.client.getClientState) {
      return this._Context.client.getClientState();
    }
  };
  /**
   * @return {?}
   */
  self.prototype.getApiVersion = function() {
    var value = this.getVersion();
    return "undefined" != value && null != value && "undefined" != value ? value.substr(0, 3) : "8.0";
  };
  /**
   * @param {string} message
   * @param {string} value
   * @param {string} dataType
   * @param {string} val
   * @param {string} max
   * @param {string} data
   * @return {undefined}
   */
  self.prototype.alertDialog = function(message, value, dataType, val, max, data) {
    this.parameterCheck(message, "Text to display in alert dialog is required.");
    this.stringParameterCheck(message, "Text of string type is required.");
    var data = {
      confirmButtonLabel : "undefined" != value && null != value ? value : null,
      text : message
    };
    var finalSizeCropProperties = {
      height : "undefined" != dataType && null != dataType && "" != dataType ? dataType : null,
      width : "undefined" != val && null != val && "" != val ? val : null
    };
    try {
      if (void 0 !== this._Xrm.Navigation && void 0 !== this._Xrm.Navigation.openAlertDialog) {
        this._Xrm.Navigation.openAlertDialog(data, finalSizeCropProperties).then(function(tx1) {
          if (null != max && "undefined" != max) {
            max(tx1);
          }
        }, function(butt) {
          if (null != data && "undefined" != data) {
            data(butt);
          }
        });
      } else {
        if (void 0 !== this._Xrm.Utility && "undefined" != this._Xrm.Utility) {
          this._Xrm.Utility.alertDialog(data.text, "undefined" != max ? max : null);
        } else {
          alert(data.text);
        }
      }
    } catch (lastErrorOutput) {
      throw new Error(lastErrorOutput);
    }
  };
  /**
   * @param {string} message
   * @param {!Object} code
   * @param {string} msg
   * @param {string} value
   * @param {string} next
   * @return {undefined}
   */
  self.prototype.errorDialog = function(message, code, msg, value, next) {
    this.parameterCheck(message, "Text to display in error dialog is required.");
    this.stringParameterCheck(message, "Text of string type is required.");
    var data = {
      errorCode : msg,
      details : code,
      message : message
    };
    if (void 0 !== this._Xrm.Navigation) {
      this._Xrm.Navigation.openErrorDialog(data).then(function(custBidObj) {
        if (null != value && "undefined" != value) {
          value(custBidObj);
        }
      }, function(encryptErr) {
        if (null != next && "undefined" != next) {
          next(encryptErr);
        }
      });
    } else {
      this.alertDialog(message);
    }
  };
  /**
   * @param {!Object} data
   * @param {string} val
   * @param {string} dataType
   * @param {string} value
   * @param {string} text
   * @param {string} max
   * @param {string} min
   * @return {undefined}
   */
  self.prototype.openConfirmDialog = function(data, val, dataType, value, text, max, min) {
    this.parameterCheck(data.title, "Text to display in alert dialog is required.");
    this.stringParameterCheck(data.title, "Text of string type is required.");
    this.parameterCheck(data.subtitle, "Text to display in alert dialog is required.");
    this.stringParameterCheck(data.subtitle, "Text of string type is required.");
    var element = {
      cancelButtonLabel : "undefined" != val && null != val ? val : "Cancel",
      confirmButtonLabel : "undefined" != dataType && null != dataType ? dataType : "OK",
      title : data.title,
      subtitle : data.subtitle
    };
    var resize = {
      height : "undefined" != value && null != value && "" != value ? value : "undefined",
      width : "undefined" != text && null != text && "" != text ? text : "undefined"
    };
    try {
      if (void 0 !== this._Xrm.Navigation && void 0 !== this._Xrm.Navigation.openConfirmDialog) {
        this._Xrm.Navigation.openConfirmDialog(element, resize).then(function(tx1) {
          if (null != max && "undefined" != max) {
            max(tx1);
          }
        }, function(nodesByDepth) {
          if (null != min && "undefined" != min) {
            min(nodesByDepth);
          }
        });
      } else {
        if (void 0 !== this._Xrm.Utility && "undefined" != this._Xrm.Utility) {
          this._Xrm.Utility.confirmDialog(element.subtitle, "undefined" != max ? max : null, "undefined" != min ? min : null);
        } else {
          alert(element.subtitle);
        }
      }
    } catch (lastErrorOutput) {
      throw new Error(lastErrorOutput);
    }
  };
  /**
   * @param {?} posy
   * @param {string} max
   * @param {string} min
   * @param {string} value
   * @param {string} count
   * @return {undefined}
   */
  self.prototype.openWebResource = function(posy, max, min, value, count) {
    if (count = "undefined" != count ? count : null, max = "undefined" != max ? max : null, min = "undefined" != min ? min : null, value = "undefined" == value || value, void 0 !== this._Xrm && void 0 !== this._Xrm.Navigation && "undefined" != this._Xrm.Navigation.openWebResource) {
      var finalSizeCropProperties = {
        height : max,
        width : min,
        openInNewWindow : value
      };
      this._Xrm.Navigation.openWebResource(posy, finalSizeCropProperties, count);
    } else {
      if (void 0 === this._Xrm || void 0 === this._Xrm.Utility || void 0 === this._Xrm.Utility.openWebResource) {
        return;
      }
      this._Xrm.Utility.openWebResource(posy, count, min, max);
    }
  };
  /**
   * @param {number} options
   * @param {number} value
   * @param {number} url
   * @param {number} lang
   * @return {undefined}
   */
  self.prototype.openEntityForm = function(options, value, url, lang) {
    if (options = void 0 !== options ? options : null, value = void 0 !== value ? value : null, url = void 0 !== url ? url : null, lang = void 0 !== lang ? lang : null, void 0 !== this._Xrm && void 0 !== this._Xrm.Navigation && "undefined" != this._Xrm.Navigation.openForm) {
      this._Xrm.Navigation.openForm(options, value, url, lang);
    } else {
      if (void 0 === this._Xrm || void 0 === this._Xrm.Utility || void 0 === this._Xrm.Utility.openEntityForm) {
        return;
      }
      var api = {
        openInNewWindow : "undefined" != options.openInNewWindow && options.openInNewWindow
      };
      this._Xrm.Utility.openEntityForm(options.entityName, options.entityId, value, api);
    }
  };
  /**
   * @return {?}
   */
  self.prototype.getFormType = function() {
    if (null != Xrm.Page && null != Xrm.Page.ui && null != Xrm.Page.ui && null != Xrm.Page.ui.getFormType) {
      return Xrm.Page.ui.getFormType();
    }
    if (null != parent.Xrm.Page && null != parent.Xrm.Page.ui && null != parent.Xrm.Page.ui && null != parent.Xrm.Page.ui.getFormType) {
      return parent.Xrm.Page.ui.getFormType();
    }
    throw new Error("ui is not available.");
  };
  /**
   * @param {?} name
   * @return {?}
   */
  self.prototype.getAttribute = function(name) {
    if (null != Xrm.Page && null != Xrm.Page && null != Xrm.Page.data && null != Xrm.Page.data && null != Xrm.Page.getAttribute && null != Xrm.Page.getAttribute) {
      return Xrm.Page.getAttribute(name);
    }
    if (null != parent.Xrm.Page && null != parent.Xrm.Page && null != parent.Xrm.Page.getAttribute && null != parent.Xrm.Page.getAttribute) {
      return parent.Xrm.Page.getAttribute(name);
    }
    throw new Error("getAttribute is not available.");
  };
  /**
   * @return {?}
   */
  self.prototype.save = function() {
    if (null != Xrm.Page && null != Xrm.Page && null != Xrm.Page.data && null != Xrm.Page.data && null != Xrm.Page.data.entity && "undefined" != Xrm.Page.data.entity && null != Xrm.Page.data.entity) {
      return Xrm.Page.data.entity.save();
    }
    if (null != parent.Xrm.Page && null != parent.Xrm.Page && null != parent.Xrm.Page.data && null != parent.Xrm.Page.data && null != parent.Xrm.Page.data.entity && "undefined" != parent.Xrm.Page.data.entity && null != parent.Xrm.Page.data.entity) {
      return parent.Xrm.Page.data.entity.save();
    }
    throw new Error("save is not available.");
  };
  /**
   * @return {?}
   */
  self.prototype.getId = function() {
    if (null != Xrm.Page && null != Xrm.Page && null != Xrm.Page.data && null != Xrm.Page.data && null != Xrm.Page.data.entity && "undefined" != Xrm.Page.data.entity && null != Xrm.Page.data.entity) {
      return Xrm.Page.data.entity.getId();
    }
    if (null != parent.Xrm.Page && null != parent.Xrm.Page && null != parent.Xrm.Page.data && null != parent.Xrm.Page.data && null != parent.Xrm.Page.data.entity && "undefined" != parent.Xrm.Page.data.entity && null != parent.Xrm.Page.data.entity) {
      return parent.Xrm.Page.data.entity.getId();
    }
    throw new Error("getId is not available.");
  };
  /**
   * @param {?} params
   * @return {?}
   */
  self.prototype.getControl = function(params) {
    if (null != Xrm.Page && null != Xrm.Page.ui && null != Xrm.Page.ui && null != Xrm.Page.ui.controls) {
      return Xrm.Page.ui.controls.get(params);
    }
    if (null != parent.Xrm.Page && null != parent.Xrm.Page.ui && null != parent.Xrm.Page.ui && null != parent.Xrm.Page.ui.controls) {
      return parent.Xrm.Page.ui.controls.get(params);
    }
    throw new Error("controls.get() is not available.");
  };
  /**
   * @param {?} url
   * @return {?}
   */
  self.prototype.getTab = function(url) {
    if (null != Xrm.Page && null != Xrm.Page.ui && null != Xrm.Page.ui && null != Xrm.Page.ui.tabs) {
      return Xrm.Page.ui.tabs.get(url);
    }
    if (null != parent.Xrm.Page && null != parent.Xrm.Page.ui && null != parent.Xrm.Page.ui && null != parent.Xrm.Page.ui.tabs) {
      return parent.Xrm.Page.ui.tabs.get(url);
    }
    throw new Error("tabs.get() is not available.");
  };
  /**
   * @param {?} isBgroundImg
   * @return {?}
   */
  self.prototype.clearFormNotification = function(isBgroundImg) {
    if (null != Xrm.Page && null != Xrm.Page.ui && null != Xrm.Page.ui && null != Xrm.Page.ui.clearFormNotification) {
      return Xrm.Page.ui.clearFormNotification(isBgroundImg);
    }
    if (null != parent.Xrm.Page && null != parent.Xrm.Page.ui && null != parent.Xrm.Page.ui && null != parent.Xrm.Page.ui.clearFormNotification) {
      return parent.Xrm.Page.ui.clearFormNotification(isBgroundImg);
    }
    throw new Error("clearFormNotification is not available.");
  };
  /**
   * @param {?} isBgroundImg
   * @param {?} stgs
   * @param {?} index
   * @return {undefined}
   */
  self.prototype.setFormNotification = function(isBgroundImg, stgs, index) {
    if (null != Xrm.Page && null != Xrm.Page.ui && null != Xrm.Page.ui && null != Xrm.Page.ui.setFormNotification) {
      Xrm.Page.ui.setFormNotification(isBgroundImg, stgs, index);
    } else {
      if (null == parent.Xrm.Page || null == parent.Xrm.Page.ui || null == parent.Xrm.Page.ui || null == parent.Xrm.Page.ui.setFormNotification) {
        throw new Error("setFormNotification is not available.");
      }
      parent.Xrm.Page.ui.setFormNotification(isBgroundImg, stgs, index);
    }
  };
  /**
   * @return {?}
   */
  self.prototype.getEntityName = function() {
    if (null != Xrm.Page && null != Xrm.Page && null != Xrm.Page.data && null != Xrm.Page.data && null != Xrm.Page.data && null != Xrm.Page.data && "undefined" != Xrm.Page.data) {
      return Xrm.Page.data.entity.getEntityName();
    }
    if (null != parent.Xrm.Page && null != parent.Xrm.Page && null != parent.Xrm.Page.data.entity && null != parent.Xrm.Page.data.entity && null != parent.Xrm.Page.data && null != parent.Xrm.Page.data && "undefined" != parent.Xrm.Page.data.entity) {
      return parent.Xrm.Page.data.entity.getEntityName();
    }
    throw new Error("getEntityName() is not available.");
  };
  /**
   * @param {string} val
   * @param {string} opt_validate
   * @return {undefined}
   */
  self.prototype.stringParameterCheck = function(val, opt_validate) {
    if ("string" != typeof val) {
      throw new Error(opt_validate);
    }
  };
  /**
   * @param {string} data
   * @param {string} linkedEntities
   * @return {undefined}
   */
  self.prototype.parameterCheck = function(data, linkedEntities) {
    if (null == data || "" === data) {
      throw new Error(linkedEntities);
    }
  };
  /** @type {function(): undefined} */
  title = self;
  /** @type {function(): undefined} */
  resultEntry_text_title.CrmJs = title;
}(IKL = IKL || {});

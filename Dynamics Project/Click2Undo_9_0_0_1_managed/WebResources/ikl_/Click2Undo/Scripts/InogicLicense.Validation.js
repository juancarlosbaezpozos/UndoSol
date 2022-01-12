'use strict';
/** @type {null} */
var _orgUniqueName = null;
/** @type {null} */
var _crmLicenseDetails = null;
/** @type {null} */
var _decryptedLicenseDetails = null;
/** @type {boolean} */
var _isOutlookClient = false;
/** @type {boolean} */
var _isOutlookOnline = true;
/** @type {null} */
var _licResults = null;
/** @type {boolean} */
var _isFreeVersion = false;
/** @type {string} */
var MSG_PRODUCT = "Invalid product name: Licensed product name: ";
/** @type {string} */
var MSG_ORGANIZATION = "Invalid organization name: Licensed organization name: ";
/** @type {string} */
var MSG_SERVER = "Invalid server name. Licensed server name: ";
/** @type {string} */
var MSG_CRMVERSION = "Product not supported for this CRM Version: ";
/** @type {string} */
var MSG_LICENSEPERIOD = "License period expired. License expired on: ";
/** @type {string} */
var MSG_MAINTENACEPERIOD = "Maintenance period expired. Maintenance expired on: ";
/** @type {string} */
var MSG_USERLICENSES = "Active user count exceeds the user license limit. Licensed user count: ";
/** @type {string} */
var MSG_FILETAMPARED = "License file has been tampered with [Tampered field: FIELDNAME] ";
/** @type {string} */
var MSG_LICENSEMISSING = "License missing for product[PRODUCTNAME] and org[ORGNAME] ";
/** @type {string} */
var MSG_DUPLICENSE = "More than 1 license found for product[PRODUCTNAME] with org[ORGNAME] ";
/** @type {string} */
var _product = "Click2Undo";
/** @type {!Object} */
var _languageLablesColl = new Object;
var _inogicLicense_CrmJs = new IKL.InogicLicense.CrmJs;
var _inogicLicense_CrmConfig = {
  APIUrl : _inogicLicense_CrmJs._ApiUrl
};
var _inogicLicense_CrmApi = new Inogic.InogicLicense.CRMWebAPI(_inogicLicense_CrmConfig);
"undefined" == typeof Inogic && (Inogic = {
  __namespace : true
}), Inogic.Validation = {}, Inogic.Validation = {
  LicenseValidation : {
    validateLicense : function open(footerButtons) {
      _product.toLowerCase().trim();
      try {
        /** @type {!Object} */
        _languageLablesColl = footerButtons;
        _orgUniqueName = _inogicLicense_CrmJs.getOrgUniqueName();
        if (null != _inogicLicense_CrmJs && null != _inogicLicense_CrmJs.getClient) {
          _isOutlookClient = _inogicLicense_CrmJs.getClient();
        }
        if (null != _inogicLicense_CrmJs && null != _inogicLicense_CrmJs.getClientState) {
          _isOutlookOnline = _inogicLicense_CrmJs.getClientState();
        }
        _licResults = {};
        if ("outlook" == _isOutlookClient.toString().toLowerCase() && "offline" == _isOutlookOnline.toString().toLowerCase() || _isOutlookClient && 0 == _isOutlookOnline) {
          /** @type {boolean} */
          _licResults.isValidLic = true;
        } else {
          _licResults = Inogic.Validation.LicenseValidation.readLicenseDetails();
        }
      } catch (_0x5546x17) {
      }
      return _licResults;
    },
    getHost : function locusOfChapter(src) {
      try {
        var _0x5546x18 = src.match(/^https?:\/\/([a-zA-Z0-9\.\-]+)(:[0-9]+)?\/?.*$/i);
        if (_0x5546x18 && _0x5546x18[1]) {
          return _0x5546x18[1];
        }
      } catch (_0x5546x17) {
      }
      return null;
    },
    readLicenseDetails : function init() {
      /** @type {null} */
      var fOpts = null;
      /** @type {null} */
      var param = null;
      /** @type {!Array} */
      var pos = new Array;
      /** @type {!Array} */
      var filenames = new Array;
      try {
        var queryOptions = {
          FetchXml : "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'><entity name='ikl_inogiclicensedetails'><attribute name='ikl_inogiclicensedetailsid' /><attribute name='ikl_name' /><attribute name='ikl_userlicenses' /><attribute name='statecode' /><attribute name='ikl_specialcase' /><attribute name='ikl_server' /><attribute name='ikl_productname' /><attribute name='ikl_organization' /><attribute name='ikl_maintenancestartdate' /><attribute name='ikl_maintenanceenddate' /><attribute name='ikl_licensetype' /><attribute name='ikl_licensestartdate' /><attribute name='ikl_licensekey' /><attribute name='ikl_licenseid' /><attribute name='ikl_licenseenddate' /><attribute name='ikl_customer' /><attribute name='ikl_crmversion' /><filter type='and'><condition attribute='ikl_productname' operator='eq' value='" + 
          _product.toLowerCase() + "' /><condition attribute='statecode' operator='eq' value='0' /></filter></entity></fetch>",
          FormattedValues : true
        };
        var foundStory = _inogicLicense_CrmApi.CustomFetch("ikl_inogiclicensedetailses", queryOptions, null);
        if (null != foundStory && 0 < foundStory.List.length) {
          fOpts = foundStory.List[0];
          /** @type {!Object} */
          _crmLicenseDetails = new Object;
          /** @type {!Object} */
          _decryptedLicenseDetails = new Object;
          if (null != fOpts.ikl_customer) {
            _crmLicenseDetails.Customer = fOpts.ikl_customer;
          }
          if (null != fOpts.ikl_productname) {
            _crmLicenseDetails.Product = fOpts.ikl_productname;
          }
          if (null != fOpts.ikl_organization) {
            _crmLicenseDetails.Organization = fOpts.ikl_organization;
          }
          if (null != fOpts.ikl_server) {
            _crmLicenseDetails.Server = fOpts.ikl_server;
          }
          if (null != fOpts.ikl_crmversion) {
            _crmLicenseDetails.CRMVersion = fOpts.ikl_crmversion;
          }
          if (null != fOpts.ikl_licensetype) {
            _crmLicenseDetails.LicenseType = fOpts.ikl_licensetype;
          }
          if (null != fOpts.ikl_licensestartdate) {
            /** @type {!Date} */
            _crmLicenseDetails.LicenseStartDate = new Date(fOpts.ikl_licensestartdate.getFullYear(), fOpts.ikl_licensestartdate.getMonth(), fOpts.ikl_licensestartdate.getDate(), 0, 0, 0, 0);
          }
          if (null != fOpts.ikl_licenseenddate) {
            /** @type {!Date} */
            _crmLicenseDetails.LicenseEndDate = new Date(fOpts.ikl_licenseenddate.getFullYear(), fOpts.ikl_licenseenddate.getMonth(), fOpts.ikl_licenseenddate.getDate(), 0, 0, 0, 0);
          }
          if (null != fOpts.ikl_maintenancestartdate) {
            /** @type {!Date} */
            _crmLicenseDetails.MaintenanceStartDate = new Date(fOpts.ikl_maintenancestartdate.getFullYear(), fOpts.ikl_maintenancestartdate.getMonth(), fOpts.ikl_maintenancestartdate.getDate(), 0, 0, 0, 0);
          }
          if (null != fOpts.ikl_maintenanceenddate) {
            /** @type {!Date} */
            _crmLicenseDetails.MaintenanceEndDate = new Date(fOpts.ikl_maintenanceenddate.getFullYear(), fOpts.ikl_maintenanceenddate.getMonth(), fOpts.ikl_maintenanceenddate.getDate(), 0, 0, 0, 0);
          }
          if (null != fOpts.ikl_userlicenses) {
            _crmLicenseDetails.UserLicenses = fOpts.ikl_userlicenses;
          }
          if (null != fOpts.ikl_specialcase) {
            _crmLicenseDetails.SpecialCase = fOpts.ikl_specialcase;
          }
          if (null != fOpts.ikl_licenseid) {
            _crmLicenseDetails.LicenseId = fOpts.ikl_licenseid;
          }
          if (null != fOpts.ikl_licensekey) {
            _crmLicenseDetails.LicenseKey = fOpts.ikl_licensekey;
          }
          var _ref = InoEncryptDecrypt.EncryptDecrypt.decryptKey(_crmLicenseDetails.LicenseKey);
          if (null != _ref) {
            if (null != (param = _ref.DecryptedValue) && null != param) {
              pos = param.split("$$");
            }
            /** @type {number} */
            var i = 0;
            for (; i < pos.length; i++) {
              if ("LicenseStartDate" == (filenames = pos[i].split("||"))[0] || "MaintenanceStartDate" == filenames[0] || "MaintenanceEndDate" == filenames[0] || "LicenseEndDate" == filenames[0]) {
                /** @type {!Date} */
                var dCurrent = new Date(filenames[1]);
                /** @type {!Date} */
                _decryptedLicenseDetails[filenames[0]] = new Date(dCurrent.getFullYear(), dCurrent.getMonth(), dCurrent.getDate(), 0, 0, 0, 0);
              } else {
                _decryptedLicenseDetails[filenames[0]] = filenames[1];
              }
            }
            _licResults = Inogic.Validation.LicenseValidation.validateProduct();
          } else {
            /** @type {boolean} */
            _licResults.isValidLic = false;
            _licResults.headerMessage = _languageLablesColl.titlemsg;
            _licResults.Message = _languageLablesColl.licmissingmsg.replace("[PRODUCTNAME]", _product).replace("ORGNAME", _orgUniqueName);
          }
        } else {
          /** @type {boolean} */
          _licResults.isValidLic = false;
          _licResults.headerMessage = _languageLablesColl.titlemsg;
          _licResults.Message = _languageLablesColl.licmissingmsg.replace("[PRODUCTNAME]", _product).replace("ORGNAME", _orgUniqueName);
        }
      } catch (to3) {
        throw new Error("readLicenseDetails " + to3);
      }
      return _licResults;
    },
    validateProduct : function create() {
      /** @type {boolean} */
      _licResults.isValidLic = true;
      try {
        if (!Inogic.Validation.LicenseValidation.validateLicenseKey()) {
          return _licResults.isValidLic = false, _licResults;
        }
        if (!Inogic.Validation.LicenseValidation.validateProductName()) {
          return _licResults.isValidLic = false, _licResults;
        }
        if (null != _crmLicenseDetails.LicenseType && "free" != _crmLicenseDetails.LicenseType.toLowerCase().trim()) {
          if (!Inogic.Validation.LicenseValidation.validateOrgName()) {
            return _licResults.isValidLic = false, _licResults;
          }
          if (!Inogic.Validation.LicenseValidation.validateServerName()) {
            return _licResults.isValidLic = false, _licResults;
          }
        } else {
          /** @type {boolean} */
          _isFreeVersion = true;
        }
        if (!Inogic.Validation.LicenseValidation.validateCRMVersion()) {
          return _licResults.isValidLic = false, _licResults;
        }
        if (!Inogic.Validation.LicenseValidation.validateLicensePeriod()) {
          return _licResults.isValidLic = false, _licResults;
        }
      } catch (jResp) {
        throw _licResults.isValidLic = false, new Error("ValidateProduct : " + jResp.Message);
      }
      return _licResults;
    },
    validateLicenseType : function push() {
      try {
        if (_crmLicenseDetails.LicenseType.toString().toLowerCase().trim().trim() != _decryptedLicenseDetails.LicenseType.toString().toLowerCase().trim().trim()) {
          _licResults.Message = _languageLablesColl.pnmsg + _decryptedLicenseDetails.LicenseType;
          /** @type {boolean} */
          _licResults.isValidLic = false;
        }
      } catch (e) {
        throw new error("Error while validate LicenseType: " + e);
      }
      return _licResults;
    },
    validateLicenseKey : function get() {
      /** @type {!Array} */
      var methodsToOverwrite = new Array("Server", "Organization", "CRMVersion", "LicenseStartDate", "LicenseEndDate", "MaintenanceStartDate", "MaintenanceEndDate", "UserLicenses", "SpecialCase", "Customer", "Product", "LicenseType", "LicenseId");
      try {
        Inogic_jQ.each(methodsToOverwrite, function(canCreateDiscussions, offset) {
          if (null != _crmLicenseDetails[offset] && null != _decryptedLicenseDetails[offset] && _crmLicenseDetails[offset].toString().toLowerCase().trim() != _decryptedLicenseDetails[offset].toString().toLowerCase().trim()) {
            /** @type {boolean} */
            _licResults.isValidLic = false;
            _licResults.headerMessage = _languageLablesColl.titlemsg;
            /** @type {string} */
            _licResults.Message = _languageLablesColl.licensetampered.replace("FIELDNAME", offset) + "<" + _crmLicenseDetails[offset].toString() + " != " + _decryptedLicenseDetails[offset].toString() + ">";
          }
        });
      } catch (to3) {
        throw new Error("validateLicenseKey  : " + to3);
      }
      return _licResults;
    },
    validateProductName : function initialize() {
      try {
        if (_crmLicenseDetails.Product.toString().toLowerCase().trim().trim() != _decryptedLicenseDetails.Product.toString().toLowerCase().trim().trim()) {
          _licResults.headerMessage = _languageLablesColl.titlemsg;
          _licResults.Message = _languageLablesColl.pnmsg + _decryptedLicenseDetails.Product;
          /** @type {boolean} */
          _licResults.isValidLic = false;
        }
      } catch (e) {
        throw new error("Error while validating ProductName: " + e);
      }
      return _licResults;
    },
    validateOrgName : function push() {
      try {
        if (null != _crmLicenseDetails.Organization && null != _orgUniqueName && _crmLicenseDetails.Organization.toString().toLowerCase().trim() != _orgUniqueName.toLowerCase().trim()) {
          _licResults.headerMessage = _languageLablesColl.titlemsg;
          _licResults.Message = _languageLablesColl.onmsg + _decryptedLicenseDetails.Organization;
          /** @type {boolean} */
          _licResults.isValidLic = false;
        }
      } catch (e) {
        throw new error("Error while validating Organization: " + e);
      }
      return _licResults;
    },
    validateServerName : function register() {
      /** @type {null} */
      var _url = null;
      /** @type {!Array} */
      var nodePlatforms = [];
      /** @type {!Array} */
      var compareTerms = [];
      try {
        if (_url = _inogicLicense_CrmJs.getClientUrl(), _url = this.getHost(_url), null != _crmLicenseDetails.Server && null != _url) {
          nodePlatforms = _crmLicenseDetails.Server.replace("--d", "").split("|");
          /** @type {number} */
          var p = 0;
          for (; p < nodePlatforms.length; p++) {
            compareTerms.push(nodePlatforms[p].toLowerCase());
          }
          if (0 == -1 < Inogic_jQ.inArray(_url.toLowerCase(), compareTerms)) {
            _licResults.headerMessage = _languageLablesColl.titlemsg;
            _licResults.Message = _languageLablesColl.snmsg + _decryptedLicenseDetails.Server;
            /** @type {boolean} */
            _licResults.isValidLic = false;
          }
        }
      } catch (_0x5546x17) {
        throw new error("Error while validating Server: " + _0x5546x17);
      }
      return _licResults;
    },
    validateCRMVersion : function init() {
      /** @type {null} */
      var response = null;
      /** @type {number} */
      var default_favicon = 0;
      try {
        if (null != (response = _inogicLicense_CrmApi.ExecuteActionSync("RetrieveVersion", null, null, null, "GET")) && "" != response) {
          default_favicon = response.Version.toString().substr(0, 3);
          if (_crmLicenseDetails.CRMVersion.toString().toLowerCase().trim().split(".")[0] != default_favicon.toString().toLowerCase().trim().split(".")[0]) {
            _licResults.headerMessage = _languageLablesColl.titlemsg;
            _licResults.Message = _languageLablesColl.cvmsg;
            /** @type {boolean} */
            _licResults.isValidLic = false;
          }
        }
      } catch (_0x5546x17) {
        throw new error("Error while validating CRMVersion: " + _0x5546x17);
      }
      return _licResults;
    },
    validateLicensePeriod : function prettyDate() {
      /** @type {null} */
      var minv = null;
      /** @type {null} */
      var maxv = null;
      /** @type {null} */
      var val = null;
      try {
        if (val = new Date, val = new Date(val.getFullYear(), val.getMonth(), val.getDate(), 0, 0, 0, 0), null != _crmLicenseDetails.LicenseStartDate && (minv = _crmLicenseDetails.LicenseStartDate), null != _crmLicenseDetails.LicenseEndDate && (maxv = _crmLicenseDetails.LicenseEndDate), maxv < minv) {
          return _licResults.headerMessage = _languageLablesColl.titlemsg, _licResults.Message = "Start date can not be greater than end date.", _licResults.isValidLic = false, _licResults;
        }
        if (val < minv) {
          return _licResults.headerMessage = _languageLablesColl.titlemsg, _licResults.Message = "Your subscription will be start on : " + (new Date(_decryptedLicenseDetails.LicenseStartDate)).toLocaleDateString(), _licResults.isValidLic = false, _licResults;
        }
        if (!(minv <= val && val <= maxv)) {
          _licResults.headerMessage = _languageLablesColl.titlemsg;
          /** @type {string} */
          _licResults.Message = _languageLablesColl.lemsg + (new Date(_decryptedLicenseDetails.LicenseEndDate)).toLocaleDateString();
          /** @type {boolean} */
          _licResults.isValidLic = false;
        }
      } catch (name) {
        throw new Exception("Error while validating license period : " + message + "" + name);
      }
      return _licResults;
    },
    __namespace : true
  }
};

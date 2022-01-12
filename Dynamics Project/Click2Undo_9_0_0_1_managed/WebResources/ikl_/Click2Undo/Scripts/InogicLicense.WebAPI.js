'use strict';
var Inogic = window.Inogic || {};
!function(canCreateDiscussions) {
  /**
   * @param {!Object} config
   * @return {?}
   */
  function CRMWebAPI(config) {
    return this.config = config, "undefined" != typeof module && module.exports ? (this.node = true, this.https = https, this.urllib = url, this._GetHttpRequest = this._GetHttpRequestHTTPS) : (this.node = false, this._GetHttpRequest = this._GetHttpRequestXMLHTTPRequest), this;
  }
  var root;
  var Codd;
  root = canCreateDiscussions.InogicLicense || (canCreateDiscussions.InogicLicense = {});
  /**
   * @param {string} category
   * @param {string} name
   * @param {!Object} value
   * @return {undefined}
   */
  CRMWebAPI.prototype._log = function(category, name, value) {
    /**
     * @param {string} name
     * @param {string} s
     * @param {!Object} oldValue
     * @return {undefined}
     */
    var logger = function log(name, s, oldValue) {
      console.log(name + ":" + s);
    };
    if (null != this.config.Log && this.config.Log.Logger) {
      logger = this.config.Log.Logger;
    }
    if (null != this.config.Log && 1 == this.config.Log[category]) {
      logger(category, name, value);
    }
  };
  /**
   * @param {!Function} func
   * @param {number} startIndex
   * @return {?}
   */
  CRMWebAPI.prototype._restParam = function(func, startIndex) {
    return startIndex = null == startIndex ? func.length - 1 : +startIndex, function() {
      /** @type {number} */
      var length = Math.max(arguments.length - startIndex, 0);
      /** @type {!Array} */
      var rest = Array(length);
      /** @type {number} */
      var i = 0;
      for (; i < length; i++) {
        rest[i] = arguments[i + startIndex];
      }
      switch(startIndex) {
        case 0:
          return func.call(this, rest);
        case 1:
          return func.call(this, arguments[0], rest);
      }
    };
  };
  /**
   * @param {!Function} test
   * @param {!Function} callback
   * @param {!Function} iterator
   * @return {undefined}
   */
  CRMWebAPI.prototype.whilst = function(test, callback, iterator) {
    if (test()) {
      var next = this._restParam(function(result, args) {
        if (result) {
          iterator(result);
        } else {
          if (test.apply(this, args)) {
            callback(next);
          } else {
            iterator.apply(null, [null].concat(args));
          }
        }
      });
      callback(next);
    } else {
      iterator(null);
    }
  };
  /**
   * @param {string} uri
   * @param {!Object} QueryOptions
   * @return {?}
   */
  CRMWebAPI.prototype.GetList = function(uri, QueryOptions) {
    var self = this;
    return new Promise(function(succes_callback, nrcEventEmitter) {
      var url = self._BuildQueryURL(uri, QueryOptions, self.config);
      self._GetHttpRequest(self.config, "GET", url, {
        headers : self._BuildQueryHeaders(QueryOptions, self.config)
      }, function(canCreateDiscussions, error) {
        if (0 != canCreateDiscussions) {
          self._log("Errors", "GetList Error:", error);
          nrcEventEmitter(error);
        } else {
          /** @type {*} */
          var data = JSON.parse(error.response, CRMWebAPI.prototype._DateReviver);
          var url = data["@odata.nextLink"];
          var recordCount = data["@odata.count"];
          var response = {
            List : data.value,
            Count : recordCount
          };
          if (null != QueryOptions && null != QueryOptions.RecordAction) {
            response.List.forEach(function(record) {
              QueryOptions.RecordAction(record);
            });
            /** @type {!Array} */
            response.List = [];
          }
          if (null != QueryOptions && null != QueryOptions.PageAction) {
            QueryOptions.PageAction(response.List);
            /** @type {!Array} */
            response.List = [];
          }
          if ("undefined" === url) {
            succes_callback(response);
          } else {
            self.whilst(function() {
              return void 0 !== url;
            }, function(readFileComplete) {
              self._GetHttpRequest(self.config, "GET", url, {
                headers : self._BuildQueryHeaders(QueryOptions, self.config)
              }, function(canCreateDiscussions, error) {
                if (0 == canCreateDiscussions) {
                  /** @type {*} */
                  data = JSON.parse(error.response, CRMWebAPI.prototype._DateReviver);
                  url = data["@odata.nextLink"];
                  response.List = response.List.concat(data.value);
                  if (null != QueryOptions && null != QueryOptions.RecordAction) {
                    response.List.forEach(function(record) {
                      QueryOptions.RecordAction(record);
                    });
                    /** @type {!Array} */
                    response.List = [];
                  }
                  if (null != QueryOptions && null != QueryOptions.PageAction) {
                    QueryOptions.PageAction(response.List);
                    /** @type {!Array} */
                    response.List = [];
                  }
                  readFileComplete(null, response.List.length);
                } else {
                  self._log("Errors", "GetList Error2", error);
                  readFileComplete("err", 0);
                }
              });
            }, function(canCreateDiscussions, isSlidingUp) {
              succes_callback(response);
            });
          }
        }
      });
    });
  };
  /**
   * @param {string} entityCollection
   * @param {string} entityID
   * @param {!Object} QueryOptions
   * @return {?}
   */
  CRMWebAPI.prototype.Get = function(entityCollection, entityID, QueryOptions) {
    var self = this;
    return new Promise(function(saveNotifs, nrcEventEmitter) {
      /** @type {null} */
      var url = null;
      url = null == entityID ? self._BuildQueryURL(entityCollection, QueryOptions, self.config) : self._BuildQueryURL(entityCollection + "(" + entityID.toString().replace(/[{}]/g, "") + ")", QueryOptions, self.config);
      self._GetHttpRequest(self.config, "GET", url, {
        headers : self._BuildQueryHeaders(QueryOptions, self.config)
      }, function(canCreateDiscussions, error) {
        if (0 != canCreateDiscussions) {
          self._log("Errors", "Get Error", error);
          nrcEventEmitter(error);
        } else {
          /** @type {*} */
          var notifications = JSON.parse(error.response, CRMWebAPI.prototype._DateReviver);
          saveNotifs(notifications);
        }
      });
    });
  };
  /**
   * @param {string} uri
   * @param {!Object} QueryOptions
   * @return {?}
   */
  CRMWebAPI.prototype.GetCount = function(uri, QueryOptions) {
    var self = this;
    return new Promise(function(obtainGETData, nrcEventEmitter) {
      var url = self._BuildQueryURL(uri + "/$count", QueryOptions, self.config);
      self._GetHttpRequest(self.config, "GET", url, {
        headers : self._BuildQueryHeaders(QueryOptions, self.config)
      }, function(canCreateDiscussions, error) {
        if (0 != canCreateDiscussions) {
          self._log("Errors", "GetCount Error", error);
          nrcEventEmitter(error);
        } else {
          /** @type {number} */
          var val = parseInt(error.response);
          obtainGETData(val);
        }
      });
    });
  };
  /**
   * @param {?} entityCollection
   * @param {?} data
   * @return {?}
   */
  CRMWebAPI.prototype.Create = function(entityCollection, data) {
    var self = this;
    return new Promise(function(saveNotifs, f_callBack) {
      var url = self.config.APIUrl + entityCollection;
      self._log("ODataUrl", url);
      self._GetHttpRequest(self.config, "POST", url, {
        data : JSON.stringify(data)
      }, function(canCreateDiscussions, res) {
        if (0 != canCreateDiscussions) {
          self._log("Errors", "Create Error", res);
          f_callBack(res);
        } else {
          saveNotifs(/\(([0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12})\)/g.exec(res.headers["odata-entityid"])[1]);
        }
      });
    });
  };
  /**
   * @param {?} entityCollection
   * @param {string} data
   * @param {?} obj
   * @param {?} index
   * @return {?}
   */
  CRMWebAPI.prototype.Update = function(entityCollection, data, obj, index) {
    var self = this;
    return new Promise(function(succes_callback, f_callBack) {
      /** @type {string} */
      var url = self.config.APIUrl + entityCollection + "(" + data.replace(/[{}]/g, "") + ")";
      self._log("ODataUrl", url);
      var payload = {
        data : JSON.stringify(obj),
        headers : {}
      };
      if (0 == index) {
        /** @type {string} */
        payload.headers["If-Match"] = "*";
      }
      self._GetHttpRequest(self.config, "PATCH", url, payload, function(canCreateDiscussions, res) {
        if (0 != canCreateDiscussions) {
          self._log("Errors", "Update Error", res);
          f_callBack(res);
        } else {
          var response = {};
          /** @type {(Array<string>|null)} */
          var parseEntityID = /\(([0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12})\)/g.exec(res.headers["odata-entityid"]);
          if (null != parseEntityID) {
            /** @type {string} */
            response.EntityID = parseEntityID[1];
          }
          succes_callback(response);
        }
      });
    });
  };
  /**
   * @param {?} entityCollection
   * @param {string} entityID
   * @return {?}
   */
  CRMWebAPI.prototype.Delete = function(entityCollection, entityID) {
    var self = this;
    return new Promise(function(saveNotifs, reallyRunRoutes) {
      /** @type {string} */
      var url = self.config.APIUrl + entityCollection + "(" + entityID.replace(/[{}]/g, "") + ")";
      self._log("ODataUrl", url);
      self._GetHttpRequest(self.config, "DELETE", url, {}, function(canCreateDiscussions, hash) {
        if (0 != canCreateDiscussions) {
          self._log("Errors", "Delete Error", hash);
          reallyRunRoutes(hash);
        } else {
          saveNotifs(true);
        }
      });
    });
  };
  /**
   * @param {?} fromEntitycollection
   * @param {string} fromEntityID
   * @param {string} navProperty
   * @param {?} toEntityCollection
   * @param {string} toEntityID
   * @return {?}
   */
  CRMWebAPI.prototype.Associate = function(fromEntitycollection, fromEntityID, navProperty, toEntityCollection, toEntityID) {
    var self = this;
    return new Promise(function(saveNotifs, reallyRunRoutes) {
      /** @type {string} */
      var url = self.config.APIUrl + fromEntitycollection + "(" + fromEntityID.replace(/[{}]/g, "") + ")/" + navProperty + "/$ref";
      self._log("ODataUrl", url);
      var payload = {
        data : JSON.stringify({
          "@odata.id" : self.config.APIUrl + toEntityCollection + "(" + toEntityID.replace(/[{}]/g, "") + ")"
        })
      };
      self._GetHttpRequest(self.config, "POST", url, payload, function(canCreateDiscussions, hash) {
        if (0 != canCreateDiscussions) {
          self._log("Errors", "Associate Error", hash);
          reallyRunRoutes(hash);
        } else {
          saveNotifs(true);
        }
      });
    });
  };
  /**
   * @param {?} fromEntitycollection
   * @param {string} fromEntityID
   * @param {string} navProperty
   * @param {?} toEntityCollection
   * @param {string} toEntityID
   * @return {?}
   */
  CRMWebAPI.prototype.DeleteAssociation = function(fromEntitycollection, fromEntityID, navProperty, toEntityCollection, toEntityID) {
    var self = this;
    return new Promise(function(saveNotifs, reallyRunRoutes) {
      /** @type {string} */
      var url = self.config.APIUrl + fromEntitycollection + "(" + fromEntityID.replace(/[{}]/g, "") + ")/" + navProperty + "/$ref";
      if (null != toEntityCollection && null != toEntityID) {
        /** @type {string} */
        url = url + ("?$id=" + self.config.APIUrl + toEntityCollection + "(" + toEntityID.replace(/[{}]/g, "") + ")");
      }
      self._log("ODataUrl", url);
      self._GetHttpRequest(self.config, "DELETE", url, {}, function(canCreateDiscussions, hash) {
        if (0 != canCreateDiscussions) {
          self._log("Errors", "DeleteAssociation Error", hash);
          reallyRunRoutes(hash);
        } else {
          saveNotifs(true);
        }
      });
    });
  };
  /**
   * @param {string} functionName
   * @param {!Object} parameters
   * @param {?} entityCollection
   * @param {string} entityID
   * @return {?}
   */
  CRMWebAPI.prototype.ExecuteFunction = function(functionName, parameters, entityCollection, entityID) {
    var self = this;
    return new Promise(function(saveNotifs, nrcEventEmitter) {
      /** @type {!Array} */
      var responseGroup = [];
      /** @type {!Array} */
      var drilldownLevelLabels = [];
      /** @type {number} */
      var default_favicon = 1;
      if (null != parameters) {
        Object.keys(parameters).forEach(function(variable) {
          var id = parameters[variable];
          responseGroup.push(variable + "=@p" + default_favicon.toString());
          if ("string" == typeof id || id instanceof String) {
            drilldownLevelLabels.push("@p" + default_favicon.toString() + "='" + id + "'");
          } else {
            drilldownLevelLabels.push("@p" + default_favicon.toString() + "=" + id);
          }
          default_favicon++;
        });
      }
      /** @type {string} */
      var url = "";
      if (null != parameters) {
        /** @type {string} */
        url = self.config.APIUrl + functionName + "(" + responseGroup.join(",") + ")?" + drilldownLevelLabels.join("&");
        if (null != entityCollection) {
          /** @type {string} */
          url = self.config.APIUrl + entityCollection + "(" + entityID.toString().replace(/[{}]/g, "") + ")" + functionName + "(" + responseGroup.join(",") + ")?" + drilldownLevelLabels.join("&");
        }
      } else {
        /** @type {string} */
        url = self.config.APIUrl + functionName + "()";
        if (null != entityCollection) {
          /** @type {string} */
          url = self.config.APIUrl + entityCollection + "(" + entityID.toString().replace(/[{}]/g, "") + ")" + functionName + "()";
        }
      }
      self._log("ODataUrl", url);
      self._GetHttpRequest(self.config, "GET", url, {}, function(canCreateDiscussions, error) {
        if (0 != canCreateDiscussions) {
          self._log("Errors", "ExecuteFunction Error", error);
          nrcEventEmitter(error);
        } else {
          /** @type {*} */
          var notifications = JSON.parse(error.response, CRMWebAPI.prototype._DateReviver);
          saveNotifs(notifications);
        }
      });
    });
  };
  /**
   * @param {string} actionName
   * @param {?} data
   * @param {?} entityCollection
   * @param {string} entityID
   * @return {?}
   */
  CRMWebAPI.prototype.ExecuteAction = function(actionName, data, entityCollection, entityID) {
    var self = this;
    return new Promise(function(saveNotifs, nrcEventEmitter) {
      var url = self.config.APIUrl + actionName;
      if (null != entityCollection) {
        /** @type {string} */
        url = self.config.APIUrl + entityCollection + "(" + entityID.toString().replace(/[{}]/g, "") + ")/" + actionName;
      }
      self._log("ODataUrl", url);
      self._GetHttpRequest(self.config, "POST", url, {
        data : JSON.stringify(data)
      }, function(canCreateDiscussions, error) {
        if (0 != canCreateDiscussions) {
          self._log("Errors", "ExecuteAction Error", error);
          nrcEventEmitter(error);
        } else {
          if ("" == error.response) {
            saveNotifs(null);
          } else {
            /** @type {*} */
            var notifications = JSON.parse(error.response, CRMWebAPI.prototype._DateReviver);
            saveNotifs(notifications);
          }
        }
      });
    });
  };
  /**
   * @param {string} pingErr
   * @param {?} fromEntitycollection
   * @param {?} issueData
   * @return {?}
   */
  CRMWebAPI.prototype.ExecuteWorkflow = function(pingErr, fromEntitycollection, issueData) {
    var self = this;
    return new Promise(function(saveNotifs, nrcEventEmitter) {
      /** @type {string} */
      var url = "";
      if (null != fromEntitycollection) {
        /** @type {string} */
        url = self.config.APIUrl + fromEntitycollection + "(" + pingErr.toString().replace(/[{}]/g, "") + ")/Microsoft.Dynamics.CRM.ExecuteWorkflow";
      }
      self._log("ODataUrl", url);
      self._GetHttpRequest(self.config, "POST", url, {
        data : JSON.stringify(issueData)
      }, function(canCreateDiscussions, error) {
        if (0 != canCreateDiscussions) {
          self._log("Errors", "ExecuteAction Error", error);
          nrcEventEmitter(error);
        } else {
          if ("" == error.response) {
            saveNotifs(null);
          } else {
            /** @type {*} */
            var notifications = JSON.parse(error.response, CRMWebAPI.prototype._DateReviver);
            saveNotifs(notifications);
          }
        }
      });
    });
  };
  /**
   * @param {string} uri
   * @param {!Object} queryOptions
   * @param {?} config
   * @return {?}
   */
  CRMWebAPI.prototype._BuildQueryURL = function(uri, queryOptions, config) {
    var key = config.APIUrl + uri;
    /** @type {!Array} */
    var responseGroup = [];
    if (null != queryOptions) {
      if (null != queryOptions.Select && responseGroup.push("$select=" + encodeURI(queryOptions.Select.join(","))), null != queryOptions.OrderBy && responseGroup.push("$orderby=" + encodeURI(queryOptions.OrderBy.join(","))), null != queryOptions.Filter && responseGroup.push("$filter=" + encodeURI(queryOptions.Filter)), null != queryOptions.Expand) {
        /** @type {!Array} */
        var expands = [];
        queryOptions.Expand.forEach(function(ex) {
          if (null != ex.Select || null != ex.Filter || null != ex.OrderBy || null != ex.Top) {
            /** @type {!Array} */
            var drilldownLevelLabels = [];
            if (null != ex.Select) {
              drilldownLevelLabels.push("$select=" + ex.Select.join(","));
            }
            if (null != ex.OrderBy) {
              drilldownLevelLabels.push("$orderby=" + ex.OrderBy.join(","));
            }
            if (null != ex.Filter) {
              drilldownLevelLabels.push("$filter=" + ex.Filter);
            }
            if (0 < ex.Top) {
              drilldownLevelLabels.push("$top=" + ex.Top);
            }
            expands.push(ex.Property + "(" + drilldownLevelLabels.join(";") + ")");
          } else {
            expands.push(ex.Property);
          }
        });
        responseGroup.push("$expand=" + encodeURI(expands.join(",")));
      }
      if (queryOptions.IncludeCount) {
        responseGroup.push("$count=true");
      }
      if (0 < queryOptions.Skip) {
        responseGroup.push("skip=" + encodeURI(queryOptions.Skip));
      }
      if (0 < queryOptions.Top) {
        responseGroup.push("$top=" + encodeURI(queryOptions.Top));
      }
      if (null != queryOptions.SystemQuery) {
        responseGroup.push("savedQuery=" + encodeURI(queryOptions.SystemQuery));
      }
      if (null != queryOptions.UserQuery) {
        responseGroup.push("userQuery=" + encodeURI(queryOptions.UserQuery));
      }
      if (null != queryOptions.FetchXml) {
        responseGroup.push("fetchXml=" + encodeURI(queryOptions.FetchXml));
      }
    }
    return 0 < responseGroup.length && (key = key + ("?" + responseGroup.join("&"))), this._log("ODataUrl", key), key;
  };
  /**
   * @param {!Object} queryOptions
   * @param {?} config
   * @return {?}
   */
  CRMWebAPI.prototype._BuildQueryHeaders = function(queryOptions, config) {
    var _0x4e7ex4 = {};
    return null != queryOptions && 1 == queryOptions.FormattedValues && (_0x4e7ex4.Prefer = 'odata.include-annotations="OData.Community.Display.V1.FormattedValue"'), _0x4e7ex4;
  };
  /**
   * @param {string} headerStr
   * @return {?}
   */
  CRMWebAPI.prototype.parseResponseHeaders = function(headerStr) {
    var headers = {};
    if (!headerStr) {
      return headers;
    }
    var headersSplit = headerStr.split("\n");
    /** @type {number} */
    var i = 0;
    for (; i < headersSplit.length; i++) {
      var line = headersSplit[i];
      var start = line.indexOf(": ");
      if (0 < start) {
        var nextPath = line.substring(0, start);
        var DATE_STRING = line.substring(start + 2);
        headers[nextPath.toLowerCase()] = DATE_STRING;
      }
    }
    return headers;
  };
  /**
   * @param {string} config
   * @param {string} method
   * @param {string} url
   * @param {!Object} data
   * @param {!Function} callback
   * @return {undefined}
   */
  CRMWebAPI.prototype._GetHttpRequestXMLHTTPRequest = function(config, method, url, data, callback) {
    var self = this;
    /** @type {!XMLHttpRequest} */
    var req = new XMLHttpRequest;
    if (req.open(method, url, true), null != config.AccessToken && req.setRequestHeader("Authorization", "Bearer " + config.AccessToken), req.setRequestHeader("Accept", "application/json"), req.setRequestHeader("OData-MaxVersion", "4.0"), req.setRequestHeader("OData-Version", "4.0"), config.callerId && req.setRequestHeader("MSCRMCallerID", config.callerId), config.CallerID && req.setRequestHeader("MSCRMCallerID", config.CallerID), 0 <= ["POST", "PUT", "PATCH"].indexOf(method) && (req.setRequestHeader("Content-Length", 
    data.data.length), req.setRequestHeader("Content-Type", "application/json")), "undefined" !== data.headers) {
      var i;
      for (i in data.headers) {
        req.setRequestHeader(i, data.headers[i]);
      }
    }
    /**
     * @return {undefined}
     */
    req.onreadystatechange = function() {
      if (4 == this.readyState) {
        /** @type {null} */
        req.onreadystatechange = null;
        if (200 <= this.status && this.status < 300) {
          callback(false, {
            response : this.response,
            headers : self.parseResponseHeaders(this.getAllResponseHeaders())
          });
        } else {
          callback(true, this);
        }
      }
    };
    if (0 <= ["POST", "PUT", "PATCH"].indexOf(method)) {
      req.send(data.data);
    } else {
      req.send();
    }
  };
  /**
   * @param {string} config
   * @param {string} method
   * @param {string} url
   * @param {!Object} payload
   * @param {!Function} callback
   * @return {undefined}
   */
  CRMWebAPI.prototype._GetHttpRequestHTTPS = function(config, method, url, payload, callback) {
    var oURL = this.urllib.parse(url);
    var options = {
      hostname : oURL.hostname,
      port : 443,
      path : oURL.path,
      method : method,
      headers : {
        Accept : "application/json",
        "OData-MaxVersion" : "4.0",
        "OData-Version" : "4.0"
      }
    };
    if (0 <= ["POST", "PUT", "PATCH"].indexOf(method) && (options.headers["Content-Length"] = payload.data.length, options.headers["Content-Type"] = "application/json"), config.callerId && (options.headers.MSCRMCallerID = config.callerId), null != config.AccessToken && (options.headers.Authorization = "Bearer " + config.AccessToken), null != payload.headers) {
      var name;
      for (name in payload.headers) {
        options.headers[name] = payload.headers[name];
      }
    }
    var store = this.https.request(options, function(req) {
      /** @type {string} */
      var adsHtml = "";
      req.setEncoding("utf8");
      req.on("data", function(imgItems) {
        adsHtml = adsHtml + imgItems;
      });
      req.on("end", function() {
        if (200 <= req.statusCode && req.statusCode < 300) {
          callback(false, {
            response : adsHtml,
            headers : req.headers
          });
        } else {
          callback(true, {
            response : adsHtml,
            headers : req.headers
          });
        }
      });
    });
    store.on("error", function(exisObj) {
      callback(true, exisObj);
    });
    if (0 <= ["POST", "PUT", "PATCH"].indexOf(method)) {
      store.write(payload.data);
    }
    store.end();
  };
  /**
   * @param {?} value
   * @param {!Date} key
   * @return {?}
   */
  CRMWebAPI.prototype._DateReviver = function(value, key) {
    var color;
    return "string" == typeof key && (color = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)Z$/["exec"](key)) ? new Date(Date.UTC(+color[1], color[2] - 1, +color[3], +color[4], +color[5], +color[6])) : key;
  };
  /**
   * @param {string} uri
   * @param {!Object} QueryOptions
   * @param {number} canCreateDiscussions
   * @return {?}
   */
  CRMWebAPI.prototype.CustomFetch = function(uri, QueryOptions, canCreateDiscussions) {
    var fakename;
    var self = this;
    /** @type {number} */
    var offset = 1;
    if (null == QueryOptions.FetchXml || null == QueryOptions.FetchXml) {
      throw new Error("Pass Fetch Xml Query");
    }
    if (fakename = QueryOptions.FetchXml, null != canCreateDiscussions && 1 == canCreateDiscussions) {
      return new Promise(function(succes_callback, nrcEventEmitter) {
        var url = self._BuildQueryURL(uri, QueryOptions, self.config);
        self._GetHttpRequest(self.config, "GET", url, {
          headers : self._BuildQueryHeaders(QueryOptions, self.config)
        }, function(canCreateDiscussions, error) {
          if (0 != canCreateDiscussions) {
            self._log("Errors", "GetList Error:", error);
            nrcEventEmitter(error);
          } else {
            /** @type {*} */
            var data = JSON.parse(error.response, CRMWebAPI.prototype._DateReviver);
            var param = 1 == data["@Microsoft.Dynamics.CRM.morerecords"] ? data["@Microsoft.Dynamics.CRM.fetchxmlpagingcookie"] : void 0;
            var recordCount = data["@odata.count"];
            var response = {
              List : data.value,
              Count : recordCount
            };
            if (null != QueryOptions && null != QueryOptions.RecordAction) {
              response.List.forEach(function(record) {
                QueryOptions.RecordAction(record);
              });
              /** @type {!Array} */
              response.List = [];
            }
            if (null != QueryOptions && null != QueryOptions.PageAction) {
              QueryOptions.PageAction(response.List);
              /** @type {!Array} */
              response.List = [];
            }
            if ("undefined" === param) {
              succes_callback(response);
            } else {
              self.whilst(function() {
                return void 0 !== param;
              }, function(readFileComplete) {
                offset++;
                var pageInfo = self.GetPagingCookie(param, offset);
                var str_trim = fakename.substring(fakename.indexOf("<fetch") + 6, fakename.length);
                /** @type {string} */
                str_trim = "<fetch " + pageInfo.pageNumber + " " + pageInfo.pageCookies + str_trim;
                /** @type {string} */
                QueryOptions.FetchXml = escape(str_trim);
                var url = self._BuildQueryURL(uri, QueryOptions, self.config);
                self._GetHttpRequest(self.config, "GET", url, {
                  headers : self._BuildQueryHeaders(QueryOptions, self.config)
                }, function(canCreateDiscussions, error) {
                  if (0 == canCreateDiscussions) {
                    /** @type {*} */
                    data = JSON.parse(error.response, CRMWebAPI.prototype._DateReviver);
                    param = data["@Microsoft.Dynamics.CRM.fetchxmlpagingcookie"];
                    response.List = response.List.concat(data.value);
                    if (null != QueryOptions && null != QueryOptions.RecordAction) {
                      response.List.forEach(function(record) {
                        QueryOptions.RecordAction(record);
                      });
                      /** @type {!Array} */
                      response.List = [];
                    }
                    if (null != QueryOptions && null != QueryOptions.PageAction) {
                      QueryOptions.PageAction(response.List);
                      /** @type {!Array} */
                      response.List = [];
                    }
                    readFileComplete(null, response.List.length);
                  } else {
                    self._log("Errors", "GetList Error2", error);
                    readFileComplete("err", 0);
                  }
                });
              }, function(canCreateDiscussions, isSlidingUp) {
                succes_callback(response);
              });
            }
          }
        });
      });
    }
    var url = self._BuildQueryURL(uri, QueryOptions, self.config);
    var result = self.CustomSyncXMLRequest(self.config, "GET", url, {
      headers : self._BuildQueryHeaders(QueryOptions, self.config)
    });
    if (null != result.response) {
      /** @type {*} */
      var data = JSON.parse(result.response, CRMWebAPI.prototype._DateReviver);
      var value = 1 == data["@Microsoft.Dynamics.CRM.morerecords"] ? data["@Microsoft.Dynamics.CRM.fetchxmlpagingcookie"] : void 0;
      var recordCount = data["@odata.count"];
      var response = {
        List : data.value,
        Count : recordCount
      };
      return null != QueryOptions && null != QueryOptions.RecordAction && (response.List.forEach(function(record) {
        QueryOptions.RecordAction(record);
      }), response.List = []), null != QueryOptions && null != QueryOptions.PageAction && (QueryOptions.PageAction(response.List), response.List = []), "undefined" === value || void 0 === value || self.whilst(function() {
        return void 0 !== value;
      }, function(indexChanger) {
        offset++;
        var items = self.GetPagingCookie(value, offset);
        var str_trim = fakename.substring(fakename.indexOf("<fetch") + 6, fakename.length);
        /** @type {string} */
        str_trim = "<fetch " + items.pageNumber + " " + items.pageCookies + str_trim;
        /** @type {string} */
        QueryOptions.FetchXml = escape(str_trim);
        var url = self._BuildQueryURL(uri, QueryOptions, self.config);
        var result = self.CustomSyncXMLRequest(self.config, "GET", url, {
          headers : self._BuildQueryHeaders(QueryOptions, self.config)
        });
        if (null != result.response) {
          /** @type {*} */
          data = JSON.parse(result.response, CRMWebAPI.prototype._DateReviver);
          value = data["@Microsoft.Dynamics.CRM.fetchxmlpagingcookie"];
          response.List = response.List.concat(data.value);
          if (null != QueryOptions && null != QueryOptions.RecordAction) {
            response.List.forEach(function(record) {
              QueryOptions.RecordAction(record);
            });
            /** @type {!Array} */
            response.List = [];
          }
          if (null != QueryOptions && null != QueryOptions.PageAction) {
            QueryOptions.PageAction(response.List);
            /** @type {!Array} */
            response.List = [];
          }
          indexChanger(null, response.List.length);
        } else {
          if (null != result.error) {
            throw new Error(result.error);
          }
        }
      }, function(canCreateDiscussions, isSlidingUp) {
        return response;
      }), response;
    }
    if (null != result.error) {
      throw new Error(result.error);
    }
  };
  /**
   * @param {string} uri
   * @param {!Object} QueryOptions
   * @return {?}
   */
  CRMWebAPI.prototype.GetSync = function(uri, QueryOptions) {
    var self = this;
    var url = self._BuildQueryURL(uri, QueryOptions, self.config);
    var req = self.CustomSyncXMLRequest(self.config, "GET", url, {
      headers : self._BuildQueryHeaders(QueryOptions, self.config)
    });
    if (null != req.response) {
      /** @type {*} */
      var _0x4e7ex8 = JSON.parse(req.response, CRMWebAPI.prototype._DateReviver);
    }
    return _0x4e7ex8;
  };
  /**
   * @param {string} config
   * @param {string} type
   * @param {!Object} src
   * @param {!Object} s
   * @return {?}
   */
  CRMWebAPI.prototype.CustomSyncXMLRequest = function(config, type, src, s) {
    /** @type {!XMLHttpRequest} */
    var req = new XMLHttpRequest;
    if (req.open(type, src, false), null != config.AccessToken && req.setRequestHeader("Authorization", "Bearer " + config.AccessToken), req.setRequestHeader("Accept", "application/json"), req.setRequestHeader("OData-MaxVersion", "4.0"), req.setRequestHeader("OData-Version", "4.0"), config.callerId && req.setRequestHeader("MSCRMCallerID", config.callerId), config.CallerID && req.setRequestHeader("MSCRMCallerID", config.CallerID), 0 <= ["POST", "PUT", "PATCH"].indexOf(type) && (req.setRequestHeader("Content-Length", 
    s.data.length), req.setRequestHeader("Content-Type", "application/json")), "undefined" !== s.headers) {
      var i;
      for (i in s.headers) {
        req.setRequestHeader(i, s.headers[i]);
      }
    }
    return 0 <= ["POST", "PUT", "PATCH"].indexOf(type) ? req.send(s.data) : req.send(), 200 <= req.status && req.status < 300 ? {
      response : req.response,
      headers : this.parseResponseHeaders(req.getAllResponseHeaders())
    } : (this._log("Errors", "Get Error", req), {
      error : JSON.parse(req.responseText).error.message
    });
  };
  /**
   * @param {string} entityCollection
   * @param {string} entityID
   * @param {!Object} QueryOptions
   * @return {?}
   */
  CRMWebAPI.prototype.CustomSyncGet = function(entityCollection, entityID, QueryOptions) {
    var self = this;
    /** @type {null} */
    var body = null;
    body = null == entityID ? self._BuildQueryURL(entityCollection, QueryOptions, self.config) : self._BuildQueryURL(entityCollection + "(" + entityID.toString().replace(/[{}]/g, "") + ")", QueryOptions, self.config);
    var data = self.CustomSyncXMLRequest(self.config, "GET", body, {
      headers : self._BuildQueryHeaders(QueryOptions, self.config)
    });
    if (null != data.response) {
      return JSON.parse(data.response, CRMWebAPI.prototype._DateReviver);
    }
    if (null != data.error) {
      throw new Error(data.error);
    }
  };
  /**
   * @param {string} s
   * @param {string} e
   * @param {?} fromEntitycollection
   * @param {string} fromEntityID
   * @param {string} time
   * @return {?}
   */
  CRMWebAPI.prototype.ExecuteActionSync = function(s, e, fromEntitycollection, fromEntityID, time) {
    var type = this.config.APIUrl + s;
    if (null != fromEntitycollection && (type = this.config.APIUrl + fromEntitycollection + "(" + fromEntityID.toString().replace(/[{}]/g, "") + ")/" + s), this._log("ODataUrl", type), null != time && "" != time) {
      var result = this.CustomSyncXMLRequest(this.config, time, type, {
        data : JSON.stringify(e)
      });
    } else {
      result = this.CustomSyncXMLRequest(this.config, "POST", type, {
        data : JSON.stringify(e)
      });
    }
    return null != result.response ? e = JSON.parse(result.response, this._DateReviver) : null != result.error ? result : void 0;
  };
  /** @type {function(!Object): ?} */
  Codd = CRMWebAPI;
  /** @type {function(!Object): ?} */
  root.CRMWebAPI = Codd;
}(Inogic = Inogic || {});

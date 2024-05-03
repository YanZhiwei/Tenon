window.tenon_pluginsMap = {
    pingTabPlugin: function (param) {
/*      console.log("Plugin called with parameter: " + param)*/
      return "Plugin called with parameter: " + param;
    }
  };
  
  window.tenon_call_plugin = async function (pluginName, param) {
    try {
      return {
        retCode: 1,
        result: await (async function (pluginName, param) {
          const plugin = window.tenon_pluginsMap[pluginName];
          if (!plugin) {
            throw new Error("Plugin: " + pluginName + " isn't registered.");
          }
          return plugin(param);
        })(pluginName, param)
      };
    } catch (error) {
      return {
        retCode: error.retCode || 500,
        message: `Error calling ${pluginName}: ${error.message}; stack: ${error.stack || ''}`
      };
    }
  };


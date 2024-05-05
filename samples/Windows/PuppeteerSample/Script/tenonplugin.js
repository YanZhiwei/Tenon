window.tenon_pluginsMap = {
  pingTab: function (param) {
    /*      console.log("Plugin called with parameter: " + param)*/
    return "Plugin called with parameter: " + param;
  },
  elementFromPoint: function (point) {
    // 边界情况处理
    if (
      !point ||
      typeof point !== "object" ||
      !("x" in point) ||
      !("y" in point)
    ) {
      throw new Error("Invalid point object");
    }

    let { x: offsetX, y: offsetY } = point;
    let foundElem = null,
      lastFrame = null;
    let totalFrameOffset = { x: 0, y: 0 };
    let doc = window.document;

    while (doc) {
      const xOfsInDoc = offsetX - totalFrameOffset.x;
      const yOfsInDoc = offsetY - totalFrameOffset.y;

      foundElem = doc.elementFromPoint(xOfsInDoc, yOfsInDoc);

      if (foundElem == null) {
        foundElem = lastFrame;
        break;
      }
      const foundElemTagName = foundElem.tagName.toLowerCase();
      if (foundElemTagName === "frame" || foundElemTagName === "iframe") {
        lastFrame = foundElem;
        const frameRect = lastFrame.getBoundingClientRect();
        totalFrameOffset.x += frameRect.left;
        totalFrameOffset.y += frameRect.top;
        doc = lastFrame.contentDocument || lastFrame.contentWindow.document;
      } else {
        break;
      }
    }

    if (foundElem == null) {
      return -1;
    }
    if (foundElem && lastFrame) {
      const elemRect = foundElem.getBoundingClientRect();
      const frameRect = lastFrame.getBoundingClientRect();

      // 性能优化：减少重复计算
      const frameWidthLessThanElem = frameRect.width < elemRect.width;
      const frameHeightLessThanElem = frameRect.height < elemRect.height;

      if (frameWidthLessThanElem || frameHeightLessThanElem)
        foundElem = lastFrame;
    }
    return window.tenon_pluginsMap.setCustomId(foundElem);
  },
  getElementRect: function (customId) {
    const elem = window.tenon_pluginsMap.getElementByCustomIdInFrames(
      customId,
      document
    );
    if (!elem) {
      console.log(`Element:${customId} not found`);
      return null;
    }
    const frame = window.tenon_pluginsMap.getFrameByCustomId(
      customId,
      document
    );
    if (!frame) {
      console.log(`Frame:${customId} not found`);
    }
    let frameLeft = 0,
      frameTop = 0;
    if (frame) {
      const frameRect = frame.getBoundingClientRect();
      frameLeft = frameRect.left;
      frameTop = frameRect.top;
      console.log(`frameRect:left:${frameRect.left},top:${frameRect.top}`);
    }
    const rect = elem.getBoundingClientRect();
    // var finalRect = {
    //   left: frameLeft - rect.left,
    //   top: frameTop - rect.top,
    //   width: rect.width,
    //   height: rect.height,
    // };
    console.log(
      `rect:left:${rect.left},top:${rect.top},width:${rect.width},height:${rect.height}}`
    );
    const finalRect = {
      left: frameLeft + rect.left,
      top: frameTop + rect.height,
      width: rect.width,
      height: rect.height,
    };
    console.log(
      `rect:left:${finalRect.left},top:${finalRect.top},width:${finalRect.width},height:${finalRect.height}}`
    );
    window.tenon_pluginsMap.drawRect(finalRect);
    return finalRect;
  },
  getElementClientCssRectangle: function (elem, doc) {
    const htmlRc = elem.getBoundingClientRect();
    let rect = {
      left: htmlRc.left,
      top: htmlRc.top,
      width: htmlRc.width,
      height: htmlRc.height,
    };
  },
  getElementByCustomIdInFrames: function (customId, doc) {
    var element = doc.querySelector('[customid="' + customId + '"]');
    if (element) {
      return element;
    } else {
      var iframes = doc.querySelectorAll("iframe");
      for (var i = 0; i < iframes.length; i++) {
        var iframeDoc =
          iframes[i].contentDocument || iframes[i].contentWindow.document;
        var elementInFrame =
          window.tenon_pluginsMap.getElementByCustomIdInFrames(
            customId,
            iframeDoc
          );
        if (elementInFrame) {
          return elementInFrame;
        }
      }
    }
    return null;
  },
  getFrameByCustomId: function (customId, doc) {
    var element = doc.querySelector('[customid="' + customId + '"]');
    if (element) {
      return null;
    } else {
      var iframes = doc.querySelectorAll("iframe");
      for (var i = 0; i < iframes.length; i++) {
        var iframeDoc =
          iframes[i].contentDocument || iframes[i].contentWindow.document;
        var elementInFrame =
          window.tenon_pluginsMap.getElementByCustomIdInFrames(
            customId,
            iframeDoc
          );
        if (elementInFrame) {
          return iframes[i];
        }
      }
    }
    return null;
  },
  setCustomId: function (elem) {
    let customId = elem.getAttribute("customid");
    if (customId) {
      return customId;
    }
    customId = Math.random().toString(16).slice(2);
    elem.setAttribute("customid", customId);
    return customId;
  },
  getPageRect: function () {
    const rect = document.body.getBoundingClientRect();
    return {
      left: rect.left,
      top: rect.top,
      width: rect.width,
      height: rect.height,
    };
  },
  getPageScroll: function () {
    return {
      scrollX: window.scrollX,
      scrollY: window.scrollY,
    };
  },
  getPageZoom: function () {
    return {
      zoom: window.devicePixelRatio,
    };
  },
  drawRect: function (rect) {
    const { left, top, width, height } = rect;
    const rectDiv = document.createElement("div");
    rectDiv.style.position = "fixed";
    rectDiv.style.left = left + "px";
    rectDiv.style.top = top + "px";
    rectDiv.style.width = width + "px";
    rectDiv.style.height = height + "px";
    rectDiv.style.border = "2px solid red";
    rectDiv.style.zIndex = 9999;
    document.body.appendChild(rectDiv);
    setTimeout(() => {
      document.body.removeChild(rectDiv);
    }, 10000);
  },
};

window.tenon_call_plugin = async function (pluginName, param) {
  try {
    return {
      code: 1,
      result: await (async function (pluginName, param) {
        const plugin = window.tenon_pluginsMap[pluginName];
        if (!plugin) {
          throw new Error("Plugin: " + pluginName + " isn't registered.");
        }
        return plugin(param);
      })(pluginName, param),
    };
  } catch (error) {
    return {
      code: error.code || 500,
      message: `Error calling ${pluginName}: ${error.message}; stack: ${
        error.stack || ""
      }`,
    };
  }
};

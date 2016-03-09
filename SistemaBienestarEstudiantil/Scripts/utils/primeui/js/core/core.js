var PUI={zindex:1000,gridColumns:{"1":"ui-grid-col-12","2":"ui-grid-col-6","3":"ui-grid-col-4","4":"ui-grid-col-3","6":"ui-grid-col-2","12":"ui-grid-col-11"},charSet:"ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",scrollInView:function(i,n){var k=parseFloat(i.css("borderTopWidth"))||0,o=parseFloat(i.css("paddingTop"))||0,m=n.offset().top-i.offset().top-k-o,j=i.scrollTop(),p=i.height(),l=n.outerHeight(true);
if(m<0){i.scrollTop(j+m)
}else{if((m+l)>p){i.scrollTop(j+m-p+l)
}}},generateRandomId:function(){var f="";
for(var e=1;
e<=10;
e++){var d=Math.floor(Math.random()*this.charSet.length);
f+=this.charSet[d]
}return f
},isIE:function(b){return(this.browser.msie&&parseInt(this.browser.version,10)===b)
},escapeRegExp:function(b){return b.replace(/([.?*+^$[\]\\(){}|-])/g,"\\$1")
},escapeHTML:function(b){return b.replace(/&/g,"&amp;").replace(/</g,"&lt;").replace(/>/g,"&gt;")
},escapeClientId:function(b){return"#"+b.replace(/:/g,"\\:")
},clearSelection:function(){if(window.getSelection){if(window.getSelection().empty){window.getSelection().empty()
}else{if(window.getSelection().removeAllRanges){window.getSelection().removeAllRanges()
}}}else{if(document.selection&&document.selection.empty){document.selection.empty()
}}},inArray:function(e,f){for(var d=0;
d<e.length;
d++){if(e[d]===f){return true
}}return false
},calculateScrollbarWidth:function(){if(!this.scrollbarWidth){if(this.browser.msie){var f=$('<textarea cols="10" rows="2"></textarea>').css({position:"absolute",top:-1000,left:-1000}).appendTo("body"),d=$('<textarea cols="10" rows="2" style="overflow: hidden;"></textarea>').css({position:"absolute",top:-1000,left:-1000}).appendTo("body");
this.scrollbarWidth=f.width()-d.width();
f.add(d).remove()
}else{var e=$("<div />").css({width:100,height:100,overflow:"auto",position:"absolute",top:-1000,left:-1000}).prependTo("body").append("<div />").find("div").css({width:"100%",height:200});
this.scrollbarWidth=100-e.width();
e.parent().remove()
}}return this.scrollbarWidth
},resolveUserAgent:function(){var g,i;
jQuery.uaMatch=function(a){a=a.toLowerCase();
var b=/(opr)[\/]([\w.]+)/.exec(a)||/(chrome)[ \/]([\w.]+)/.exec(a)||/(version)[ \/]([\w.]+).*(safari)[ \/]([\w.]+)/.exec(a)||/(webkit)[ \/]([\w.]+)/.exec(a)||/(opera)(?:.*version|)[ \/]([\w.]+)/.exec(a)||/(msie) ([\w.]+)/.exec(a)||a.indexOf("trident")>=0&&/(rv)(?::| )([\w.]+)/.exec(a)||a.indexOf("compatible")<0&&/(mozilla)(?:.*? rv:([\w.]+)|)/.exec(a)||[];
var c=/(ipad)/.exec(a)||/(iphone)/.exec(a)||/(android)/.exec(a)||/(windows phone)/.exec(a)||/(win)/.exec(a)||/(mac)/.exec(a)||/(linux)/.exec(a)||/(cros)/i.exec(a)||[];
return{browser:b[3]||b[1]||"",version:b[2]||"0",platform:c[0]||""}
};
g=jQuery.uaMatch(window.navigator.userAgent);
i={};
if(g.browser){i[g.browser]=true;
i.version=g.version;
i.versionNumber=parseInt(g.version)
}if(g.platform){i[g.platform]=true
}if(i.android||i.ipad||i.iphone||i["windows phone"]){i.mobile=true
}if(i.cros||i.mac||i.linux||i.win){i.desktop=true
}if(i.chrome||i.opr||i.safari){i.webkit=true
}if(i.rv){var h="msie";
g.browser=h;
i[h]=true
}if(i.opr){var j="opera";
g.browser=j;
i[j]=true
}if(i.safari&&i.android){var f="android";
g.browser=f;
i[f]=true
}i.name=g.browser;
i.platform=g.platform;
this.browser=i;
$.browser=i
},getGridColumn:function(b){return this.gridColumns[b+""]
},executeFunctionByName:function(i){var h=[].slice.call(arguments).splice(1),l=window,j=i.split("."),k=j.pop();
for(var g=0;
g<j.length;
g++){l=l[j[g]]
}return l[k].apply(this,h)
},resolveObjectByName:function(f){if(f){var h=f.split(".");
for(var j=0,g=h.length,i=window;
j<g;
++j){i=i[h[j]]
}return i
}else{return null
}},getCookie:function(b){return $.cookie(b)
},setCookie:function(d,f,e){$.cookie(d,f,e)
},deleteCookie:function(c,d){$.removeCookie(c,d)
}};
PUI.resolveUserAgent();
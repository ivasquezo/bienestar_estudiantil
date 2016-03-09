(function(){$.widget("primeui.puigrowl",{options:{sticky:false,life:3000,messages:null,appendTo:document.body},_create:function(){var b=this.element;
this.originalParent=this.element.parent();
b.addClass("ui-growl ui-widget");
if(this.options.appendTo){b.appendTo(this.options.appendTo)
}if(this.options.messages){this.show(this.options.messages)
}},show:function(d){var c=this;
this.element.css("z-index",++PUI.zindex);
this.clear();
if(d&&d.length){$.each(d,function(b,a){c._renderMessage(a)
})
}},clear:function(){var d=this.element.children("div.ui-growl-item-container");
for(var c=0;
c<d.length;
c++){this._unbindMessageEvents(d.eq(c))
}d.remove()
},_renderMessage:function(f){var e='<div class="ui-growl-item-container ui-state-highlight ui-corner-all ui-helper-hidden" aria-live="polite">';
e+='<div class="ui-growl-item ui-shadow">';
e+='<div class="ui-growl-icon-close fa fa-close" style="display:none"></div>';
e+='<span class="ui-growl-image fa fa-2x '+this._getIcon(f.severity)+" ui-growl-image-"+f.severity+'"/>';
e+='<div class="ui-growl-message">';
e+='<span class="ui-growl-title">'+f.summary+"</span>";
e+="<p>"+(f.detail||"")+"</p>";
e+='</div><div style="clear: both;"></div></div></div>';
var d=$(e);
this._bindMessageEvents(d);
d.appendTo(this.element).fadeIn()
},_removeMessage:function(b){b.fadeTo("normal",0,function(){b.slideUp("normal","easeInOutCirc",function(){b.remove()
})
})
},_bindMessageEvents:function(e){var f=this,d=this.options.sticky;
e.on("mouseover.puigrowl",function(){var a=$(this);
if(!a.is(":animated")){a.find("div.ui-growl-icon-close:first").show()
}}).on("mouseout.puigrowl",function(){$(this).find("div.ui-growl-icon-close:first").hide()
});
e.find("div.ui-growl-icon-close").on("click.puigrowl",function(){f._removeMessage(e);
if(!d){window.clearTimeout(e.data("timeout"))
}});
if(!d){this._setRemovalTimeout(e)
}},_unbindMessageEvents:function(f){var g=this,h=this.options.sticky;
f.off("mouseover.puigrowl mouseout.puigrowl");
f.find("div.ui-growl-icon-close").off("click.puigrowl");
if(!h){var e=f.data("timeout");
if(e){window.clearTimeout(e)
}}},_setRemovalTimeout:function(e){var f=this;
var d=window.setTimeout(function(){f._removeMessage(e)
},this.options.life);
e.data("timeout",d)
},_getIcon:function(b){switch(b){case"info":return"fa-info-circle";
break;
case"warn":return"fa-warning";
break;
case"error":return"fa-close";
break;
default:return"fa-info-circle";
break
}},_setOption:function(d,c){if(d==="value"||d==="messages"){this.show(c)
}else{$.Widget.prototype._setOption.apply(this,arguments)
}},_destroy:function(){this.clear();
this.element.removeClass("ui-growl ui-widget");
if(this.options.appendTo){this.element.appendTo(this.originalParent)
}}})
})();
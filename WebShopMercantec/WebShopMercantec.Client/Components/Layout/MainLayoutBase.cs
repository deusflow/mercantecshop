using Microsoft.AspNetCore.Components;

namespace WebShopMercantec.Client.Components.Layout;

public class MainLayoutBase : LayoutComponentBase
{
	protected RenderFragment? PageBody => Body;
}


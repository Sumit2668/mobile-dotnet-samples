﻿using System;
using System.Collections.Generic;
using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Shared;
using Shared.Droid;

namespace AdvancedMap.Droid
{
	public class BaseMapSectionMenu : BaseMenu
	{
		public EventHandler<OptionEventArgs> SelectionChange;

		List<BaseMapSectionMenuItem> views = new List<BaseMapSectionMenuItem>();
		public MapSwitch Texts3D, Buildings3D;

		LinearLayout contentContainer;

		List<Section> items;
		public List<Section> Items
		{
			get { return items; }
			set
			{
				items = value;

				foreach (Section section in items)
				{
					BaseMapSectionMenuItem view = new BaseMapSectionMenuItem(context);
					view.Section = section;
					views.Add(view);
					contentContainer.AddView(view);
				}

				Texts3D = GetSwitch("3D Texts", "texts3d");
				Buildings3D = GetSwitch("3D Buildings", "buildings3d");

				contentContainer.AddView(Texts3D);
				contentContainer.AddView(Buildings3D);
			}
		}

		MapSwitch GetSwitch(string text, string name)
		{
			int screenWidth = context.Resources.DisplayMetrics.WidthPixels;

			int padding = (int)(screenWidth * 0.05);
			int width = (int)(screenWidth / 2);

			MapSwitch view = new MapSwitch(context);
			view.Text = text;
			view.ParameterName = name;

			var parameters = new LinearLayout.LayoutParams(width, (int)(width / 5), 1);
			parameters.LeftMargin = padding;
			parameters.TopMargin = (int)(padding * 0.75f);

			view.LayoutParameters = parameters;

			return view;
		}

		Context context;

		public BaseMapSectionMenu(Context context) : base(context)
		{
			this.context = context;

			contentContainer = new LinearLayout(context);
			contentContainer.LayoutParameters = new ViewGroup.LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
			contentContainer.Orientation = Orientation.Vertical;

			AddView(contentContainer);

			SetBackgroundColor(Color.Argb(130, 0, 0, 0));
		}

		public bool LanguageChoiceEnabled
		{
			set
			{
				BaseMapSectionMenuItem language = views.Find(item => item.section.Type == SectionType.Language);

				if (language == null)
				{
					return;
				}

				if (value)
				{
					language.Enabled = true;
				}
				else
				{
					language.Enabled = false;
				}
			}
		}

		BaseMapSectionLabel current;
		BaseMapSectionLabel currentLanguage;

		public override bool OnTouchEvent(MotionEvent e)
		{
			MotionEventActions action = e.Action;

			int x = (int)e.GetX();
			int y = (int)e.GetY();

			if (e.Action == MotionEventActions.Up)
			{
				bool isInBox = false;

				foreach (BaseMapSectionMenuItem item in views) {

					Rect outerRect = item.HitRect;

					if (outerRect.Contains(x, y))
					{
						isInBox = true;
						int headerHeight = item.HeaderHeight;

						if (!item.Enabled)
						{
							return true;
						}

						foreach (BaseMapSectionLabel option in item.Options) 
						{
							if (option.GetGlobalRect(headerHeight, outerRect).Contains(x, y))
							{
								if (item.Section.Type == SectionType.Language)
								{
									if (currentLanguage != null)
									{
										currentLanguage.Normalize();
									}

									option.Highlight();

									currentLanguage = option;
								}
								else
								{
									if (current != null)
									{
										current.Normalize();
									}

									option.Highlight();

									current = option;
								}

								if (SelectionChange != null)
								{
									SelectionChange(null, new OptionEventArgs { Section = item.Section, Option = option });
								}
							}
						}
					}
				}

				if (!isInBox)
				{
					Hide();
				}
			}

			return true;
		}

		public void SetInitialItem(Section section)
		{
			foreach (BaseMapSectionMenuItem view in views)
			{
				if (view.Section.Equals(section))
				{
					if (section.Type == SectionType.Language)
					{
						currentLanguage = view.SetFirstItemActive();
					}
					else
					{
						current = view.SetFirstItemActive();
					}
				}
			}
		}

	}
}


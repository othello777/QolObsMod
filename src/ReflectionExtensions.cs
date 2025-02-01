using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

public static class ReflectionExtensions
{
	public static object GetMemberValue(this object obj, string memberName)
	{
		MemberInfo memberInfo = GetMemberInfo(obj, memberName);
		bool flag = memberInfo == null;
		if (flag)
		{
			throw new Exception("memberName");
		}
		bool flag2 = memberInfo is PropertyInfo;
		object obj2;
		if (flag2)
		{
			obj2 = memberInfo.As<PropertyInfo>().GetValue(obj, null);
		}
		else
		{
			bool flag3 = memberInfo is FieldInfo;
			if (!flag3)
			{
				throw new Exception();
			}
			obj2 = memberInfo.As<FieldInfo>().GetValue(obj);
		}
		return obj2;
	}

	public static object SetMemberValue(this object obj, string memberName, object newValue)
	{
		MemberInfo memberInfo = GetMemberInfo(obj, memberName);
		bool flag = memberInfo == null;
		if (flag)
		{
			throw new Exception("memberName");
		}
		object memberValue = obj.GetMemberValue(memberName);
		bool flag2 = memberInfo is PropertyInfo;
		object obj2;
		if (flag2)
		{
			memberInfo.As<PropertyInfo>().SetValue(obj, newValue, null);
			obj2 = memberValue;
		}
		else
		{
			bool flag3 = memberInfo is FieldInfo;
			if (!flag3)
			{
				throw new Exception();
			}
			memberInfo.As<FieldInfo>().SetValue(obj, newValue);
			obj2 = memberValue;
		}
		return obj2;
	}

	private static MemberInfo GetMemberInfo(object obj, string memberName)
	{
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(obj.GetType().GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy));
		List<PropertyInfo> list2 = list.Where((PropertyInfo i) => i != null).ToList<PropertyInfo>();
		bool flag = list2.Count != 0;
		MemberInfo memberInfo;
		if (flag)
		{
			memberInfo = list2[0];
		}
		else
		{
			List<FieldInfo> list3 = new List<FieldInfo>();
			list3.Add(obj.GetType().GetField(memberName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy));
			List<FieldInfo> list4 = list3.Where((FieldInfo i) => i != null).ToList<FieldInfo>();
			bool flag2 = list4.Count != 0;
			if (flag2)
			{
				memberInfo = list4[0];
			}
			else
			{
				memberInfo = null;
			}
		}
		return memberInfo;
	}

	[DebuggerHidden]
	private static T As<T>(this object obj)
	{
		return (T)((object)obj);
	}
}

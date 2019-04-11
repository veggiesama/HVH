using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Extensions {

public static class ExtensionMethods
{
	public static string DebugToString<TKey,TValue>
		(this IDictionary<TKey,TValue> dictionary)
	{
		if (dictionary == null)
			throw new ArgumentNullException("dictionary");

		var items = from kvp in dictionary
					select kvp.Key + "=" + kvp.Value;

		return "{" + string.Join(",", items) + "}";
	}

}

}
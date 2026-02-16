using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
	public class ResponseMessage<T>
	{
		public T _entity;
		public bool _status;
		public int _statusCode;
		public string _message;
        public string _error;
        public long _timeStamp;
		public string _payload;
		public RequestMessage _request;
        public string _access;
        public string _refresh;

        public override string ToString ()
		{
			return string.Format ("[ResponseMessage] _statusCode = {0}, _entity = {1}, _message = {2}", _statusCode, _entity, _message);
		}	
	}	
}

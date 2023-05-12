using System.IO;
using System.Text;
using System.Text.Json;

#region auxiliary_classes
public class FA
{
    public string states { get; set; }
    public string input_symbols { get; set; }
    public Dictionary<string, Dictionary<string, string>> transitions { get; set; }
    public string initial_state { get; set; }
    public string final_states { get; set; }

}
public class mystate
{
    public string Name { get; set; }
    public Dictionary<string, List<mystate>> next_state;

    public mystate(string name)
    {
        Name = name;
        next_state = new Dictionary<string, List<mystate>>();
    }

    public void add_neighbor(string str, mystate sta)
    {
        if (next_state.ContainsKey(str))
        {
            next_state[str].Add(sta);
        }
        else
        {
            next_state.Add(str, new List<mystate>());
            add_neighbor(str, sta);
        }
    }
}
public class toJson
{
    public string states { get; set; }
    public string input_symbols { get; set; }
    public Dictionary<string, Dictionary<string, string>> transitions { get; set; }
    public string initial_state { get; set; }
    public string final_states { get; set; }

    public toJson(string input_symbol,
    Dictionary<string, Dictionary<string, string>> tran,
    string initial, List<string> final_state)
    {
        #region func_states
        var sb = new StringBuilder();
        sb.Append("{");
        foreach(var t in tran)
        {
            sb.Append($"'{t.Key}'");
            sb.Append(",");
        }
        sb.Remove(sb.Length -1,1);
        sb.Append("}");
        #endregion
        states = sb.ToString();
        input_symbols = input_symbol;
        transitions = tran;
        initial_state = initial;
        #region func_final
        sb = new StringBuilder();
        sb.Append("{");
        foreach(var t in final_state)
        {
            sb.Append($"'{t}',");
        }
        sb.Remove(sb.Length -1,1);
        sb.Append("}");
        #endregion
        final_states = sb.ToString();
    }
}
#endregion

class NFAtoDFA
{
    #region main
    public static void Main()
    {
        var execute = initial();
        var nfa = analyze(execute);
        var dfa = nfatodfa(nfa);
        var finish = new toJson(execute.input_symbols, dfa.Item1,
        execute.initial_state, dfa.Item2);
        string json = JsonSerializer.Serialize(finish);
        File.WriteAllText("myout_2.json", json);
    }
    #endregion

    #region main fuction
    private static (Dictionary<string,Dictionary<string,string>> , List<string>) nfatodfa((mystate[], List<string>, List<mystate>, mystate) t)
    {
        var newstates = landatrans(t.Item1);
        var res = new Dictionary<string, Dictionary<string, string>>();
        var stack =  new Dictionary<string, List<mystate>>();
        var final_state = new List<string>();
        stack.Add(t.Item4.Name, new List<mystate>(){t.Item4});
        while (stack.Count != 0)
        {
            var iter = stack.First();
            if (!res.ContainsKey(iter.Key))
            {
            res.Add(iter.Key, new Dictionary<string, string>());
            foreach (var theta in t.Item2)
            {
                var list_next_theta = new List<mystate>();
                foreach (var s in iter.Value)
                {
                    if (s.next_state.ContainsKey(theta))
                    {
                        foreach(var n in s.next_state[theta])
                        {
                            if (!list_next_theta.Contains(n))
                            {
                                list_next_theta.Add(n);
                            }
                        }
                    }
                }
                string name = myname(list_next_theta);
                if (!stack.ContainsKey(name))
                {
                    stack.Add(name, list_next_theta);
                    if (check_final_state(list_next_theta,t.Item3) &&
                    !final_state.Contains(name))
                    {
                        final_state.Add(name);
                    }
                }
                res[iter.Key].Add(theta, name);
            }
            }
            stack.Remove(iter.Key);
        }
        return (res, final_state);
    }

    #endregion

    #region auxiliary fuctions(check_final_state & myname)
    private static bool check_final_state(List<mystate> list_next_theta,
    List<mystate> final)
    {
        bool res = false;
        foreach (var f in final)
        {
            if(list_next_theta.Contains(f))
            {
                res = true;
                return res;
            }
        }
        return res;
    }

    private static string myname(List<mystate> list_next_theta)
    {
        if (list_next_theta.Count == 0)
        {
            return "TRAP";
        }
        var sb = new StringBuilder();
        var llist = list_next_theta.OrderBy(x => Convert.ToInt32(x.Name.Last()));
        foreach(var l in llist)
        {
            sb.Append(l.Name);
        }
        return sb.ToString();
    }
    #endregion

    #region auxiliary functions(mystr_1_2_3 & landatrans)
    private static List<string> mystr_1(string str)
    {
        var sb = new StringBuilder();
        sb.Append(str);
        sb.Remove(str.Length - 1, 1);
        sb.Remove(0, 1);
        return sb.ToString().Split(",").Select(x => $"{x[1]}{x[2]}").ToList();
    }
    private static List<string> mystr_2(string str)
    {
        var sb = new StringBuilder();
        sb.Append(str);
        sb.Remove(str.Length - 1, 1);
        sb.Remove(0, 1);
        return sb.ToString().Split(",").Select(x => $"{x[1]}").ToList();
    }
    private static mystate[] mystr_3(Dictionary<string, mystate> all,
    Dictionary<string, Dictionary<string, string>> transitions)
    {
        mystate[] res = new mystate[all.Count];
        var iter = transitions.ToList();
        for (int i = 0; i < iter.Count; i++)
        {
            var inner_dict = iter[i].Value.ToList();
            for (int j = 0; j < inner_dict.Count; j++)
            {
                var trans = inner_dict[j].Key;
                if (trans == "")
                {
                    trans = "landa";
                }
                var state_list = mystr_1(inner_dict[j].Value);
                for (int k = 0; k < state_list.Count; k++)
                {
                    all[iter[i].Key].add_neighbor(trans, all[state_list[k]]);
                }
            }
            res[i] = all[iter[i].Key];
        }
        return res;
    }
    #endregion

    #region auxiliary fuctions(landatrans, combine)
    private static mystate[] landatrans(mystate[] state)
    {
        var nstate = state.ToList();
        for(int i=0 ; i<nstate.Count ;i++)
        {
            foreach (var t in nstate[i].next_state)
            {
                if (t.Key == "landa")
                {
                    foreach (var v in t.Value)
                    {
                        combine(nstate[i], v);
                        nstate[i].next_state.Remove("landa");
                        // if (nstate.IndexOf(v) < i)
                        // {
                        //     i--;
                        // }
                        // nstate.Remove(v);
                        // foreach (var n in nstate)
                        // {
                        //     foreach (var val in n.next_state.Values)
                        //     {
                        //         val.Remove();
                        //         val.Add(nstate[i]);
                        //     }
                        // }
                    }
                }
            }
        }
        return nstate.ToArray();

    }

    private static void combine(mystate state_1, mystate state_2)
    {
        var sb = new StringBuilder();
        if (Convert.ToInt32(state_1.Name.Last()) > Convert.ToInt32(state_2.Name.Last()))
        {
            sb.Append(state_2.Name);
            sb.Append(state_1.Name);
        }
        else
        {
            sb.Append(state_1.Name);
            sb.Append(state_2.Name);
        }
        state_1.Name = sb.ToString();
        foreach (var s2 in state_2.next_state)
        {
            if (!state_1.next_state.ContainsKey(s2.Key))
            {
                state_1.next_state.Add(s2.Key, s2.Value);
            }
            else
            {
                foreach (var a in s2.Value)
                {
                    if (!state_1.next_state[s2.Key].Contains(a))
                    {
                        state_1.next_state[s2.Key].Add(a);
                    }
                }
            }
        }
    }
    #endregion

    #region initial&analyze functions

    private static FA initial()
    {
        string text =
        File.ReadAllText("C:\\git\\TLA01-Projects\\samples\\phase1-sample\\in\\input2.json");
        FA test = JsonSerializer.Deserialize<FA>(text);
        return test;
    }
    
    private static (mystate[], List<string>, List<mystate>, mystate)
    analyze(FA execute)
    {
        Dictionary<string, mystate> all = new Dictionary<string, mystate>();
        List<mystate> final_state = new List<mystate>();
        mystate start;
        List<string> n_states = mystr_1(execute.states);
        List<string> n_final = mystr_1(execute.final_states);
        List<string> n_symbols = mystr_2(execute.input_symbols);
        for (int i = 0; i < n_states.Count; i++)
        {
            all.Add(n_states[i], new mystate(n_states[i]));
        }
        for (int i = 0; i < n_final.Count; i++)
        {
            final_state.Add(all[n_final[i]]);
        }
        start = all[execute.initial_state];
        mystate[] res = mystr_3(all, execute.transitions);
        return (res, n_symbols, final_state, start);
    }
    #endregion
}